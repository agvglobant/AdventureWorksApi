using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TodoApi.Models
{
    public class ApiDbContextFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {        
        public ApiDbContext CreateDbContext(string[] args)
        {
            var connectionString = @"Server=DESKTOP-MKHGKA1;Database=AdventureWorks20141;User Id=sa;Password=123;";
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>()
                                    .UseSqlServer(connectionString).Options;
            
            return new ApiDbContext(optionsBuilder);
        }
    }
}