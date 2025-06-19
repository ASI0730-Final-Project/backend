using gigu_back_end.User.Domain.Models.Entities;
using Gigs.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace gigu_back_end.Shared.Infrastructure.Persistence.Configuration
{
    public class GigUContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User.Domain.Models.Entities.User> Users { get; set; }
        public DbSet<Gig> Gigs { get; set; }
        public DbSet<Pull> Pulls { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User Configuration
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

            // Gig Configuration
            // En OnModelCreating:
            builder.Entity<Gig>(entity =>
            {
                entity.ToTable("Gigs");
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Image)
                    .IsRequired()
                    .HasColumnType("LONGTEXT"); 

                entity.Property(g => g.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(g => g.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(g => g.SellerId) 
                    .IsRequired();

                entity.Property(g => g.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(g => g.Tags)
                    .HasConversion(
                        v => string.Join(',', v), 
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    )
                    .HasColumnType("TEXT");

                entity.Property(g => g.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(g => g.ExtraFeatures)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    )
                    .HasColumnType("TEXT");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                // Índices
                entity.HasIndex(g => g.SellerId);
                entity.HasIndex(g => g.Category);
                entity.HasIndex(g => new { g.IsResponsive, g.CustomAnimations });
            });

            // Pull Configuration
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

            // Chat Configuration
            builder.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).IsRequired().HasMaxLength(1000);
                entity.Property(c => c.SentAt).IsRequired();
            });
        }
    }
}
