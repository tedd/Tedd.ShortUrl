using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tedd.ShortUrl.Database
{
    [Index(propertyNames: nameof(Key), IsUnique = true)]
    public class UrlItem
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)]
        [Required]
        public string Key { get; set; }
        [Required]
        [MaxLength(512)]
        public string Url { get; set; }
        public DateTime? Expires { get; set; }
        public string? Metadata { get; set; }
    }
}