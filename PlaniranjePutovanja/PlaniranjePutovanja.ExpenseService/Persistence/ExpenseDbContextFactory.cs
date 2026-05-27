using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Persistence
{
    public sealed class ExpenseDbContextFactory : IDesignTimeDbContextFactory<ExpenseDbContext>
    {
        private const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=TravelPlanner_ExpenseDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public ExpenseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExpenseDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new ExpenseDbContext(optionsBuilder.Options);
        }
    }
}
