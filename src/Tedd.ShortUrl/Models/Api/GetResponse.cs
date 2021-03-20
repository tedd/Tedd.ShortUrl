using System;

namespace Tedd.ShortUrl.Models.Api
{
    public class GetResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string ShortUrl { get; set; }
        public string Url { get; set; }
        public DateTime? Expires { get; set; }
        public string? Metadata { get; set; }
    }
}