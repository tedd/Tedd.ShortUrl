using Microsoft.AspNetCore.Mvc;

using System;
using System.Xml.Linq;

namespace Tedd.ShortUrl.Models.Google
{
    public class ReCaptchaVerifyResponse
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string Hostname { get; set; }
        [ModelBinder(Name = "error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
