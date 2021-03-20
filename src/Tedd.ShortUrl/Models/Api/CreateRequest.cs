using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tedd.ShortUrl.Models.Api
{
    public class CreateRequest
    {
        public string AuthToken { get; set; }
        public string Url { get; set; }
        public DateTime? Expires { get; set; }
        public string? Metadata { get; set; }
    }
}
