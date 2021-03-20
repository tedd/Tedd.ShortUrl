using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tedd.ShortUrl.Database
{
    [Index(nameof(AuthToken))]
    public class AuthTokenItem
    {
        [Key]
        public int Id { get; set; }
        public string AuthToken { get; set; }
        public bool CanCreate { get; set; }
        public bool CanGet { get; set; }
    }
}