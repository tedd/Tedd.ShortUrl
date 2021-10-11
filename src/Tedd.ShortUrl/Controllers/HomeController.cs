using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Tedd.ShortUrl.Database;
using Tedd.ShortUrl.Models;
using Tedd.ShortUrl.Models.Home;
using Tedd.ShortUrl.Models.Settings;
using Tedd.ShortUrl.Services;
using Tedd.ShortUrl.Utils;

namespace Tedd.ShortUrl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShortUrlService _shortUrlService;
        private readonly AppSettings _config;

        public HomeController(ILogger<HomeController> logger, ShortUrlService shortUrlService, IOptions<AppSettings> config)
        {
            _logger = logger;
            _shortUrlService = shortUrlService;
            _config = config.Value;
        }

        [Route("")]
        public async Task<IActionResult> Index(IndexPostRequest request)
        {
            request.Url = request.Url?.Trim();
            var model = new IndexViewModel()
            {
                GoogleAnalyticsId = _config.Google.AnalyticsId
            };
            if (!string.IsNullOrEmpty(request.Url))
            {
                // Check if URL is valid
                if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var url))
                {
                    model.Text = $"Invalid URL format: {request.Url}";
                    return View(model);
                }

                var urlItem = new UrlItem() { Url = request.Url };
                try
                {
                    await _shortUrlService.CreateAsync(urlItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Creating short url for: {url}");
                    model.ErrorMessage = "Error creating short url. Check error logs for details.";
                }

                var shortUrl = Helpers.GetShortUrl(Request, urlItem.Key);
                model.Text = $"New short url created";
                model.Url = shortUrl;
            }

            return View(model);
        }

        [Route("{key}")]
        public async Task<IActionResult> Index(string key)
        {
            // Look up URL and redirect
            UrlItem? item = null;
            try
            {
                item = await _shortUrlService.GetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fetching short url for: {key}");
                return View(new IndexViewModel()
                {
                    GoogleAnalyticsId = _config.Google.AnalyticsId,
                    ErrorMessage = "Error creating short url. Check error logs for details."
                });
            }

            if (item == null || item.Expires != null && item.Expires < DateTime.Now)
            {
                // Unknown key
                return View(new IndexViewModel
                {
                    GoogleAnalyticsId = _config.Google.AnalyticsId,
                    Text = "Unknown short url"
                });
            }

            // Found it, redirect user there
            return RedirectPermanent(item.Url);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
