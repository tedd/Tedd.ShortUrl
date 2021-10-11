using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tedd.ShortUrl.Models.Home;
using Tedd.ShortUrl.Models.Settings;

namespace Tedd.ShortUrl.Utils
{
    public static class Helpers
    {
        public static string GetBaseUrl(HttpRequest request) => $"{request.Scheme}://{request.Host}{request.PathBase}";
        public static string GetShortUrl(HttpRequest request, string key) => $"{Helpers.GetBaseUrl(request)}/{key}";
        public static string GetRandomKey(CreateSettings settings) => (new Random()).NextString(settings.KeyChars, settings.KeyLength);
    }
}
