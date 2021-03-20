namespace Tedd.ShortUrl.Models.Settings
{
    public class CreateSettings
    {
        public int KeyLength { get; set; } = 5;
        public string KeyChars { get; set; } = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}