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

        public DbSet<Item> Items { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(@"FileName=db\Zhang.db");
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(a =>
            {
                a.HasOne(b=>b.CreateUser).WithMany().HasForeignKey(b=>b.CreateUserId).IsRequired();
                a.HasOne(b=>b.ModifyUser).WithMany().HasForeignKey(b=>b.ModifyUserId);
                a.HasOne(b=>b.DelUser).WithMany().HasForeignKey(b=>b.DelUserId);
            });
            modelBuilder.Entity<Item>(a => {
                a.HasOne(b => b.Image).WithMany(b=>b.Items).IsRequired(false).HasForeignKey(b=>b.ImageId);
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
            });
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