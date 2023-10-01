using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LQ.Wx.Zhang.DAL
{
    public class ZhangDb:DbContext
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(b=>b.AddDebug());
        public ZhangDb()
        {
            this.Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(@"FileName=db\Zhang.db");
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(a => {
                a.Property(a=>a.CreateUserId).IsRequired(false);
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired(false);
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
            });
        }
        public object Tag { get; set; }="";
    }
}