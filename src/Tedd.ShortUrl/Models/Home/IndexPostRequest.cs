using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Tedd.ShortUrl.Models.Home
{
    public class IndexPostRequest
{
        public string? Url { get; set; }
        [ModelBinder(Name = "g-recaptcha-response")]
        public string? G_Recaptcha_Response { get; set; }
    }
}
