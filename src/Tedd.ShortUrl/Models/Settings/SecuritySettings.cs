using System.Collections.Generic;

namespace Tedd.ShortUrl.Models.Settings
{
    public class SecuritySettings
    {
        public List<string>? AuthenticationTokens { get; set; }
        public bool RequireTokenForCreate { get; set; } = true;
        public bool RequireTokenForGet { get; set; } = true;
    }
}
