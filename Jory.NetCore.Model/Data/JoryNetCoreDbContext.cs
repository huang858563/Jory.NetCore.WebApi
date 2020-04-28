using Jory.NetCore.Model.Entities;
using Microsoft.EntityFrameworkCore;

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
