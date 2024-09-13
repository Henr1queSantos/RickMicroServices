using Mango.Services.CuponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CuponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options)
        {

        }

        public DbSet<Cupon> Cupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cupon>().HasData(new Cupon
            {
                CuponId = 1,
                CuponCode = "10OFF",
                DiscountAmount = 10,
                MinAmount = 20
            });

            modelBuilder.Entity<Cupon>().HasData(new Cupon
            {
                CuponId = 2,
                CuponCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40
            });
        }
    }
}
