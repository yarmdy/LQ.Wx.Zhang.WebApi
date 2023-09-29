using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.Common;
using LQ.Wx.Zhang.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LQ.Wx.Zhang.WebApi.Controllers
{
    [ApiController,Authorize]
    public abstract class BaseController<T, PageReqT> :ControllerBase where T : BaseEntity where PageReqT : PageReq
    {
        public BaseController() {
            this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty).Where(a=> a.PropertyType.BaseType != null && a.PropertyType.BaseType.IsGenericType && a.PropertyType.BaseType.GetGenericTypeDefinition() == typeof(BaseBll<,>)).ToList().ForEach(p => {
                p.SetValue(this, Common.HttpContext.Current!.RequestServices.GetRequiredService(p.PropertyType));
            });
        }
        #region ÊôÐÔ
        public int CurrentUserId => Bll.CurrentUserId;
        public string? CurrentUserName => Bll.CurrentUserName;
        public string? CurrentAccount => Bll.CurrentAccount;
        #endregion

        #region Ðé·½·¨

        #endregion
        public abstract BaseBll<T, PageReqT> Bll { get; }
        public UserBll UserBll { get; set; } = default!;
    }
}