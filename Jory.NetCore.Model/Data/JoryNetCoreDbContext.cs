using Jory.NetCore.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jory.NetCore.Model.Data
{
    public class JoryNetCoreDbContext : DbContext
    {
        public JoryNetCoreDbContext(DbContextOptions<JoryNetCoreDbContext> options) : base(options)
        {
        }

        public DbSet<ADUserT> ADUserT { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            //modelBuilder.Entity<Company>().Property(x => x.Introduction).HasMaxLength(500);

            //modelBuilder.Entity<ADUserT>().HasData(
            //    new ADUserT()
            //    {

            //        LoginName = "admin",
            //        UserName = "管理員",
            //        LoginPwdHash = "sha1:64000:18:u2lwvCr8q8xna3QuArfbl4mDy9ZInoQF:IJGWNhEKctB6yfnuJe3Scs6z",
            //        Role = "admin"
            //    }
            //);
        }
    }
}
