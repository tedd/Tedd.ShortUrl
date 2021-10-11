namespace Tedd.ShortUrl.Models.Home
{
    public class IndexViewModel
    {
        public string? Text { get; set; }
        public string? Url { get; set; }
        public string? ErrorMessage { get; internal set; }
    }
}
