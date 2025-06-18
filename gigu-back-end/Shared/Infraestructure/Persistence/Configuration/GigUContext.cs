using gigu_back_end.User.Domain.Models.Entities;
using Gigs.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace gigu_back_end.Shared.Infrastructure.Persistence.Configuration
{
    public class GigUContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User.Domain.Models.Entities.User> Users { get; set; }
        public DbSet<Pull> Pulls { get; set; } // 👈 NUEVO DbSet

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User Entity Configuration
            builder.Entity<User.Domain.Models.Entities.User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Lastname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Image)
                    .HasMaxLength(255);
            });

            // Pull Entity Configuration
            builder.Entity<Pull>(entity =>
            {
                entity.ToTable("Pulls");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.SellerId).IsRequired();
                entity.Property(p => p.BuyerId);
                entity.Property(p => p.GigId).IsRequired();
                entity.Property(p => p.PriceInit).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(p => p.PriceUpdate).HasColumnType("decimal(10,2)");
                entity.Property(p => p.State).IsRequired().HasMaxLength(20);
            });
        }
    }
}
