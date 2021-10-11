using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

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
        private static readonly ConcurrentDictionary<string, byte> _keys = new(Environment.ProcessorCount * 2, 10240);
        private readonly IMemoryCache _cache;

        public ShortUrlService(ILogger<HomeController> logger, ShortUrlDbContext dbContext, IOptions<AppSettings> config, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config.Value;
            _cache = memoryCache;

            // On first visit we fill key lookup cache
            lock (_keys)
            {
                if (_keys.Count == 0)
                {
                    var keys = _dbContext.Urls.Select(s => s.Key).ToArray();
                    foreach (var key in keys)
                        _keys.TryAdd(key, 0);
                }
            }
        }

        internal async Task<UrlItem?> GetAsync(string key) => await
            _cache.GetOrCreateAsync<UrlItem>(key, async entry => {

                var item = await _dbContext.Urls.FirstOrDefaultAsync(u => u.Key == key);
                if (item == null)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_config.Cache.NegativeCacheTimeoutSeconds);
                return item;
                });


        internal async Task CreateAsync(UrlItem urlItem)
        {
            //  Find free random id
            string key;
            do
            {
                key = Helpers.GetRandomKey(_config.Create);
            } while (_keys.ContainsKey(key));

            urlItem.Key = key;

            // Add item to key cache + database
            _keys.TryAdd(key, 0);
            _dbContext.Urls.Add(urlItem);
            await _dbContext.SaveChangesAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions();
            _cache.Set<UrlItem>(key, urlItem, cacheEntryOptions);

        }


    }
}
