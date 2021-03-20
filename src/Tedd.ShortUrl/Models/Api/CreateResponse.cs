using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tedd.ShortUrl.Models.Api
{
    public class CreateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public string Key { get; set; }
        public string ShortUrl { get; set; }
        public DateTime? Expires { get; set; }
    }
}
