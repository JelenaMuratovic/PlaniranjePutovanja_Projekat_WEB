using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Persistence
{
    public sealed class TravelDbContextFactory : IDesignTimeDbContextFactory<TravelDbContext>
    {
        private const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=TravelPlanner_TravelDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public TravelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new TravelDbContext(optionsBuilder.Options);
        }
    }
}
