using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Persistence
{
    /// <summary>
    /// EF Core ce da koristi ovu klasu za kreiranje instance AppDbContext-a tokom migracija i drugih dizajnerskih operacija
    /// </summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=TravelPlanner_AuthDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
