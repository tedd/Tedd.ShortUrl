using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Tedd.ShortUrl.Database;
using Tedd.ShortUrl.Models.Api;
using Tedd.ShortUrl.Models.Settings;
using Tedd.ShortUrl.Services;
using Tedd.ShortUrl.Utils;

namespace Tedd.ShortUrl.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShortUrlDbContext _dbContext;
        private readonly ShortUrlService _shortUrlService;
        private readonly AppSettings _config;

        public ApiController(ILogger<HomeController> logger, ShortUrlDbContext dbContext, IOptions<AppSettings> config, ShortUrlService shortUrlService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _shortUrlService = shortUrlService;
            _config = config.Value;
        }

        [Route("Create")]
        [HttpPost]
        public async Task<CreateResponse> Create([FromBody] CreateRequest request)
        {
            // Security: Request will only be allowed if "RequireTokenForGet" is false OR request.AuthToken is in list or in database
            if (_config.Security.RequireTokenForGet
                // Not in allowed list in config
                && _config.Security.AuthenticationTokens != null
                && !_config.Security.AuthenticationTokens.Contains(request.AuthToken)
                // And not in database
                && !_dbContext.AuthTokens.Any(w => w.AuthToken == request.AuthToken && w.CanCreate)
                )
            {
                // Not accepted
                return new CreateResponse()
                {
                    Success = false,
                    ErrorMessage = "Invalid authentication token"
                };
            }

            // Create item
            var urlItem = new UrlItem()
            {
                Key = Helpers.GetRandomKey(_config.Create),
                Expires = request.Expires,
                Url = request.Url,
                Metadata = request.Metadata
            };

            try
            {
                await _shortUrlService.CreateAsync(urlItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Creating short url for: {request.Url}");
                return new CreateResponse()
                {
                    Success = false,
                    ErrorMessage = "Error: Have admin check logs for more information."
                };
            }

            return new CreateResponse()
            {
                Success = true,
                Key = urlItem.Key,
                ShortUrl = Helpers.GetShortUrl(Request, urlItem.Key),
                Expires = request.Expires
            };
        }

        [Route("Get")]
        [HttpPost] // Post because token will be logged in weblog if not
        public async Task<GetResponse> Get([FromBody] GetRequest request)
        {
            // Security: Request will only be allowed if "RequireTokenForGet" is false OR request.AuthToken is in list or in database
            if (_config.Security.RequireTokenForGet
                // Not in allowed list in config
                && _config.Security.AuthenticationTokens != null
                && !_config.Security.AuthenticationTokens.Contains(request.AuthToken)
                // And not in database
                && !_dbContext.AuthTokens.Any(w => w.AuthToken == request.AuthToken && w.CanGet)
                )
            {
                // Not accepted
                return new GetResponse()
                {
                    Success = false,
                    ErrorMessage = "Invalid authentication token"
                };
            }

            UrlItem? urlItem;
            try
            {
                urlItem = await _shortUrlService.GetAsync(request.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Fetching short url for: {request.Key}");
                return new GetResponse()
                {
                    Success = false,
                    ErrorMessage = "Error: Have admin check logs for more information."
                };
            }

            if (urlItem == null)
            {
                // We could not find this entry
                return new GetResponse()
                {
                    Success = false,
                    ErrorMessage = "Not found"
                };
            }

            // Success, return data
            return new GetResponse()
            {
                Url = urlItem.Url,
                ShortUrl = Helpers.GetShortUrl(Request, urlItem.Key),
                Expires = urlItem.Expires,
                Metadata = urlItem.Metadata,
                Success = true
            };
        }
    }
}