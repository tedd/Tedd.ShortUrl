using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Tedd.ShortUrl.Controllers;
using Tedd.ShortUrl.Database;
using Tedd.ShortUrl.Models.Settings;
using Tedd.ShortUrl.Utils;

namespace Tedd.ShortUrl.Services
{
    public class ShortUrlService
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShortUrlDbContext _dbContext;
        private readonly AppSettings _config;
        private readonly IMemoryCache _cache;

        public ShortUrlService(ILogger<HomeController> logger, ShortUrlDbContext dbContext, IOptions<AppSettings> config, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config.Value;
            _cache = memoryCache;

        }

        internal async Task<UrlItem?> GetAsync(string key) => await
            _cache.GetOrCreateAsync<UrlItem>(key, async entry =>
            {

                var item = await _dbContext.Urls.FirstOrDefaultAsync(u => u.Key == key);
                if (item == null)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_config.Cache.NegativeCacheTimeoutSeconds);
                entry.Size = 1;
                return item;
            });


        internal async Task CreateAsync(UrlItem urlItem)
        {
            // Add item to database
            _dbContext.Urls.Add(urlItem);

            var success = false;
            Exception? lastException = null;
            // Try max 10 times before giving up
            string key = null;
            for (var i = 0; i < 10; i++)
            {
                //  Create random id
                key = Helpers.GetRandomKey(_config.Create);

                // Try to add item to database
                urlItem.Key = key;
                try
                {
                    await _dbContext.SaveChangesAsync();
                    success = true;
                }
                catch (DbUpdateException dbUpdateException) when ((dbUpdateException.InnerException as SqlException)?.Number == 2601)
                {
                    // Already exists: We need to retry
                    _logger.LogInformation(dbUpdateException, $"Collision creating ShortUrl: {urlItem.Key}");
                    lastException = dbUpdateException;
                    continue;
                } 
                catch (Exception exception)
                {
                    // Not sure what is wrong, log it and try again
                    _logger.LogError(exception, $"Creating new ShortUrl: {JsonSerializer.Serialize(urlItem)}");
                    lastException = exception;
                    continue;
                }
            }

            // Not success: Throw last error
            if (!success)
                throw lastException!;

            // Success: Add item to recent used cache since it will probably be used immediately
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.Size = 1;
            _cache.Set<UrlItem>(key, urlItem, cacheEntryOptions);
            
        }


    }
}
