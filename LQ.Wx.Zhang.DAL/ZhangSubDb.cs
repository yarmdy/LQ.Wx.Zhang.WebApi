using LQ.Wx.Zhang.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LQ.Wx.Zhang.DAL
{
    public class ZhangSubDb: DbContext
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(b=>b.AddDebug());
        private string? _openid;
        public ZhangSubDb()
        {
            var name = HttpContext.Current?.User?.Identity?.Name;
            
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            _openid = name.Split("|")[1];
            this.Database.EnsureCreated();
        }
        public ZhangSubDb(string openid)
        {
            if (string.IsNullOrEmpty(openid))
            {
                return;
            }
            _openid = openid;
            this.Database.EnsureCreated();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var openid = _openid;
            if (!Directory.Exists(@$"db\sub\{openid}"))
            {
                Directory.CreateDirectory(@$"db\sub\{openid}");
            }
            optionsBuilder.UseSqlite(@$"FileName=db\sub\{openid}\{openid}.db");
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
            modelBuilder.Entity<ItemClass>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
            });
            modelBuilder.Entity<Item>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
                a.HasOne(b=>b.ItemClass).WithMany(b=>b.Items).HasForeignKey(b=>b.ClassId);
            });
            modelBuilder.Entity<Stock>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
                a.HasOne(b => b.Item).WithMany().HasForeignKey(b => b.ItemId).IsRequired();
            });
            modelBuilder.Entity<StockDetail>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
                a.HasOne(b => b.Item).WithMany().HasForeignKey(b => b.ItemId).IsRequired();
                a.HasOne(b => b.BeginData).WithOne(b => b.StockDetail).HasForeignKey<StockDetail>(b => b.BeginId);
                a.HasOne(b => b.Order).WithMany().HasForeignKey(b => b.OrderId);
                a.HasOne(b => b.OrderDetail).WithOne(b => b.StockDetail).HasForeignKey<StockDetail>(b=>b.OrderDetailId);
            });
            modelBuilder.Entity<Client>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
            });
            modelBuilder.Entity<InOutOrder>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
                a.HasOne(b => b.Client).WithMany(b=>b.Orders).HasForeignKey(b => b.ClientId).IsRequired();
            });
            modelBuilder.Entity<InOutOrderDetail>(a => {
                a.HasOne(b => b.CreateUser).WithMany().HasForeignKey(b => b.CreateUserId).IsRequired();
                a.HasOne(b => b.ModifyUser).WithMany().HasForeignKey(b => b.ModifyUserId);
                a.HasOne(b => b.DelUser).WithMany().HasForeignKey(b => b.DelUserId);
                a.HasOne(b => b.inOutOrder).WithMany().HasForeignKey(b=>b.OrderId).IsRequired();
                a.HasOne(b => b.Item).WithMany().HasForeignKey(b => b.ItemId).IsRequired();
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