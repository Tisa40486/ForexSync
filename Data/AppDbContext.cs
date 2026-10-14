using ForexSync.Models;
using Microsoft.EntityFrameworkCore;
namespace ForexSync.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => new
                {
                    x.Date,
                    x.Currency
                }).IsUnique();

                entity.Property(x => x.Currency)
                    .HasMaxLength(3)
                    .IsRequired();

                entity.Property(x => x.Rate)
                    .HasPrecision(20, 8);

                entity.Property(x => x.Date)
                    .HasColumnType("date");

                entity.Property(x => x.UpdatedAt)
                    .HasColumnType("datetime2");
            });
        }
    }
}