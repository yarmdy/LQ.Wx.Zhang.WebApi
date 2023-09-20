using LQ.Wx.Zhang.DAL;

namespace LQ.Wx.Zhang.BLL
{
    public class ItemBll:BaseBll<Item>
    {
        public ItemBll(ZhangDb db) : base(db)
        {
        }

        public Item Create(Item entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.CreateUserId = 0;
            Db.Add(entity);
            Db.SaveChanges();
            return entity;
        }
    }
}