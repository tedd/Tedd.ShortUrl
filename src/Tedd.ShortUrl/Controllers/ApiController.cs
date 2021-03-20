using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tedd.ShortUrl.Database;
using Tedd.ShortUrl.Models.Api;
using Tedd.ShortUrl.Models.Settings;
using Tedd.ShortUrl.Utils;

namespace Tedd.ShortUrl.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShortUrlDbContext _dbContext;
        private readonly AppSettings _config;

        public ApiController(ILogger<HomeController> logger, ShortUrlDbContext dbContext, IOptions<AppSettings> config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config.Value;
        }

        [Route("Create")]
        [HttpPost]
        public CreateResponse Create([FromBody] CreateRequest request)
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
            var item = new UrlItem()
            {
                Key = Helpers.GetRandomKey(_config.Create),
                Expires = request.Expires,
                Url = request.Url,
                Metadata = request.Metadata
            };
            _dbContext.Urls.Add(item);
            _dbContext.SaveChanges();

            return new CreateResponse()
            {
                Success = true,
                Key = item.Key,
                ShortUrl = Helpers.GetShortUrl(Request, item.Key),
                Expires = request.Expires
            };
        }

        [Route("Get")]
        [HttpPost] // Post because token will be logged in weblog if not
        public GetResponse Get([FromBody] GetRequest request)
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

            var item = _dbContext.Urls.FirstOrDefault(w => w.Key == request.Key);
            if (item == null)
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
                Url = item.Url,
                ShortUrl = Helpers.GetShortUrl(Request, item.Key),
                Expires = item.Expires,
                Metadata = item.Metadata,
                Success = true
            };
        }
    }
}