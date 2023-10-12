using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.BLL.Model;
using LQ.Wx.Zhang.Common;
using LQ.Wx.Zhang.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace LQ.Wx.Zhang.WebApi.Controllers
{
    [ApiController, Authorize]
    [Route("[controller]")]
    public class ItemController : BaseController<Item,ItemReq.Page>
    {
        public ItemBll CurBll { get; set; } = default!;

        public override BaseBll<Item, ItemReq.Page> Bll => CurBll;
        [HttpGet("ByCode")]
        public Response<Item?, object> ByCode(string code)
        {
            return CurBll.ByCode(code);
        }
        [HttpGet("Get/{id:int}")]
        public Response<Item?, object, object, object> GetDetail(int id)
        {
            return CurBll.GetDetail(id,null);
        }
        [HttpPost("Add")]
        public Response<Item?, object> Add(Item model)
        {
            return CurBll.Add(model);
        }
        [HttpPost("edit")]
        public Response<Item?, object> Edit(Item model)
        {
            return CurBll.Modify(model);
        }
        [HttpPost("list")]
        public Response<List<Item>, object, object, object> GetPageList(ItemReq.Page req)
        {
            return CurBll.GetPageList(req);
        }
    }
}