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
using Tedd.ShortUrl.Utils;

namespace Tedd.ShortUrl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShortUrlDbContext _dbContext;
        private readonly AppSettings _config;

        public HomeController(ILogger<HomeController> logger, ShortUrlDbContext dbContext, IOptions<AppSettings> config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config.Value;
        }

        [Route("")]
        public IActionResult Index(IndexPostRequest request)
        {
            request.Url = request.Url?.Trim();
            var model = new IndexViewModel();
            if (!string.IsNullOrEmpty(request.Url))
            {
                // Check if URL is valid
                if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var url))
                {
                    model.Text = $"Invalid URL format: {request.Url}";
                    return View(model);
                }

                //  Create URL
                var key = Helpers.GetRandomKey(_config.Create);
                var item = new UrlItem()
                {
                    Key = key,
                    Url = request.Url
                };
                _dbContext.Urls.Add(item);
                _dbContext.SaveChanges();

                var shortUrl = Helpers.GetShortUrl(Request, key);
                model.Text = $"New short url created";
                model.Url = shortUrl;
            }

            return View(model);
        }



        [Route("{key}")]
        public IActionResult Index(string key)
        {
            // Look up URL and redirect
            var item = _dbContext.Urls.FirstOrDefault(u => u.Key == key);

            if (item == null || item.Expires != null && item.Expires < DateTime.Now)
            {
                // Unknown key
                var model = new IndexViewModel
                {
                    Text = "Unknown short url"
                };
                return View(model);
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
