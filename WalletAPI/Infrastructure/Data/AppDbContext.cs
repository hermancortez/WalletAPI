using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(w => w.Balance)
                      .HasPrecision(18, 2); // Precisión adecuada para dinero
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Amount)
                      .HasPrecision(18, 2); // Precisión adecuada para dinero
            });
        }
    }
}
