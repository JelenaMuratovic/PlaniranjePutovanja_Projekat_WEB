using Microsoft.EntityFrameworkCore;
using PlaniranjePutovanja.AuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Konfiguracija entiteta User
            modelBuilder.Entity<User>(entity =>
            {
                // primarni kljuc
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(36);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email_Unique");
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);                
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()"); // SQL funkcija za trenutno vreme
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.RefreshToken).HasMaxLength(500); 
                entity.Property(e => e.RefreshTokenExpiryTime).IsRequired(false);
                entity.ToTable("Users");
            });
        }
    }
}
