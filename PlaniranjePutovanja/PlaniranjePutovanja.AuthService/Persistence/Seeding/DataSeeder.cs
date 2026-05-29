using Microsoft.EntityFrameworkCore;
using PlaniranjePutovanja.AuthService.Helpers.Passwords;
using PlaniranjePutovanja.AuthService.Models;
using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Persistence.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUserAsync(AppDbContext context, IPasswordHasher passwordHasher)
        {
            // Provera da li vec imamo admina u bazi
            var adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin);

            if (!adminExists)
            {
                string plainTextPassword = "admin123!";
                string hashedPassword = passwordHasher.Hash(plainTextPassword);

                var superAdmin = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "admin@planiranjeputovanja.com",
                    PasswordHash = hashedPassword,
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(superAdmin);
                await context.SaveChangesAsync();
            }
        }
    }
}
