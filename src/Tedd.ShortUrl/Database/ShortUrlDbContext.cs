using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tedd.ShortUrl.Database
{
    public class ShortUrlDbContext : DbContext
    {
        public ShortUrlDbContext(DbContextOptions<ShortUrlDbContext> options): base(options)
        {
            
        }
        public DbSet<UrlItem> Urls { get; set; }
        public DbSet<AuthTokenItem> AuthTokens { get; set; }

        
    }
}
