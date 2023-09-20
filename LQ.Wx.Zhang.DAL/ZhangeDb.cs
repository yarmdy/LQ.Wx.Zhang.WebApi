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
                a.HasKey(b => b.Id);
            });
            modelBuilder.Entity<Item>(a => { 
                a.HasKey(b=>b.Id);
                a.HasOne(b => b.Image).WithMany(b=>b.Items).IsRequired(false).HasForeignKey(b=>b.ImageId);
            });
            
        }
    }
}