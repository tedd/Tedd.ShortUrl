namespace Tedd.ShortUrl.Models.Google
{
    public class ReCaptchaVerifyRequest
    {
        public string secret { get; set; }
        public string response { get; set; }
    }
}
