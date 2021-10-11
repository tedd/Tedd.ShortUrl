namespace Tedd.ShortUrl.Models.Settings
{
    public class CacheSettings
    {
        public int ItemLimit { get; set; }
        public int NegativeCacheTimeoutSeconds { get; set; }
    }
}