﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tedd.ShortUrl.Models.Settings
{
    public class AppSettings
    {
        public DatabaseSettings Database { get; set; }
        public UrlSettings Url { get; set; }
        public SecuritySettings Security { get; set; }
        public CacheSettings Cache { get; set; }
        public GoogleSettings Google { get; set; }
    }
}
