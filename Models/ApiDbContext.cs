using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public ApiDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.BusinessEntityID)
                .HasName("PrimaryKey_EmployeeID");
            */
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<BusinessEntity> BusinessEntities { get; set; }
        public virtual DbSet<Person> People { get; set; }

    }
}