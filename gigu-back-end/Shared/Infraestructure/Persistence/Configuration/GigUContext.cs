using gigu_back_end.User.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace gigu_back_end.Shared.Infrastructure.Persistence.Configuration
{
    public class GigUContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User.Domain.Models.Entities.User> Users { get; set; }

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
                    .IsUnique(); // Email debe ser único

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Image)
                    .HasMaxLength(255);

            });
        }
    }
}