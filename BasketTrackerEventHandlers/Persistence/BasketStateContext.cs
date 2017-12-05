using System;
using Microsoft.EntityFrameworkCore;
using Persistence.Model;

namespace Persistence
{
    public class BasketStateContext : DbContext
    {
        public DbSet<BasketItem> BasketItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //     optionsBuilder.UseSqlServer(@"Server=sqlserver;Database=BasketItems;User=sa;Password=password123;");
              optionsBuilder.UseSqlServer(@"Server=.;Database=BasketItems;Trusted_Connection=True;");
        }
    }
}
