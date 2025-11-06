using ASKPA.API.Model;
using Microsoft.EntityFrameworkCore;

namespace ASKPA.API.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<clsBusinessInfo> Business {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<clsBusinessInfo>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
