using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tedd.ShortUrl.Models.Settings
{
    public class AppSettings
    {
        public DatabaseSettings Database { get; set; }
        public CreateSettings Create { get; set; }
        public SecuritySettings Security { get; set; }
    }

    public class SecuritySettings
    {
        public List<string>? AuthenticationTokens { get; set; }
        public bool RequireTokenForCreate { get; set; } = true;
        public bool RequireTokenForGet { get; set; } = true;
    }
}
