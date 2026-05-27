using Microsoft.EntityFrameworkCore;
using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Persistence
{
    public class TravelDbContext : DbContext
    {
        public DbSet<Travel> Travels { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Checklist> Checklists { get; set; }

        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Travel configuration
            modelBuilder.Entity<Travel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(36);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(36);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Budget).HasPrecision(10, 2);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                entity.HasMany(e => e.Destinations)
                    .WithOne(d => d.Travel)
                    .HasForeignKey(d => d.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Checklists)
                    .WithOne(c => c.Travel)
                    .HasForeignKey(c => c.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Travels");
            });

            // Destination configuration
            modelBuilder.Entity<Destination>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(36);
                entity.Property(e => e.TravelId).IsRequired().HasMaxLength(36);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Latitude).HasPrecision(10, 8);
                entity.Property(e => e.Longitude).HasPrecision(11, 8);
                entity.Property(e => e.DaysSpent).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.Activities)
                    .WithOne(a => a.Destination)
                    .HasForeignKey(a => a.DestinationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Destinations");
            });

            // Activity configuration
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(36);
                entity.Property(e => e.DestinationId).IsRequired().HasMaxLength(36);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ActivityDate).IsRequired();
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                entity.ToTable("Activities");
            });

            // Checklist configuration
            modelBuilder.Entity<Checklist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(36);
                entity.Property(e => e.TravelId).IsRequired().HasMaxLength(36);
                entity.Property(e => e.Item).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsCompleted).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                entity.ToTable("Checklists");
            });
        }

    }
}
