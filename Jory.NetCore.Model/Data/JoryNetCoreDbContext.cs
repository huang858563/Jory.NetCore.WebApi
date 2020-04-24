using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Jory.NetCore.Model.Entities;

namespace Jory.NetCore.Model.Data
{
    public class JoryNetCoreDbContext:DbContext
    {
        public JoryNetCoreDbContext(DbContextOptions<JoryNetCoreDbContext> options) : base(options) { }

        public DbSet<ADUserT> ADUserT { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            //modelBuilder.Entity<Company>().Property(x => x.Introduction).HasMaxLength(500);

            //modelBuilder.Entity<Employee>().Property(x => x.EmpNo).IsRequired().HasMaxLength(10);
            //modelBuilder.Entity<Employee>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            //modelBuilder.Entity<Employee>().Property(x => x.LastName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<ADUserT>().HasData(
                new ADUserT()
                {
                    UserName = "admin",
                    Password = "123456",
                    PasswordHash = "xxxx",
                    Role = "admin"
                }
            );
        }
    }
}
