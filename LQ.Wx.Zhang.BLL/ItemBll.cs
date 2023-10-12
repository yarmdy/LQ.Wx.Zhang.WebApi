using LQ.Wx.Zhang.DAL;

namespace LQ.Wx.Zhang.BLL
{
    public class ItemBll:BaseBll<Item,ItemReq.Page>
    {
        public Response<Item?,object> ByCode(string code)
        {
            var res = new Response<Item?,object>();
            var item = GetFirstOrDefault(a=>a.Where(b=>b.Code==code));
            if (item == null)
            {
                res.msg = "产品不存在";
            }
            res.data=item;
            return res;
        }
        public override bool AddBefore(out string errorMsg, Item model, Item inModel)
        {
            errorMsg = string.Empty;
            var old =  GetFirstOrDefault(a => a.Where(b => b.Code == model.Code), false);
            if (old != null)
            {
                errorMsg = "产品已存在";
                return false;
            }
            return true;
        }
    }
}