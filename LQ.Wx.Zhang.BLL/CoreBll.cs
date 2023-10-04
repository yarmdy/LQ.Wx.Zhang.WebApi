using LQ.Wx.Zhang.DAL;
using LQ.Wx.Zhang.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LQ.Wx.Zhang.BLL.Model;

namespace LQ.Wx.Zhang.BLL
{
    public class CoreBll
    {
        public CoreBll() {
            Context = Common.HttpContext.Current!.RequestServices.GetRequiredService<ZhangDb>();
        }
        public User? GetUser(string openId,string sessionKey,bool create=false) {
            var user = Context.Users.FirstOrDefault(a=>a.OpenId==openId);
            if (user==null && !create)
            {
                return null;
            }
            if (user == null)
            {
                user = new User { };
                user.CreateTime = DateTime.Now;
                user.OpenId = openId;
                Context.Users.Add(user);
            }
            user.SessionKey = sessionKey;
            Context.SaveChanges();
            return user;
        }
        public User? SetUserInfo(int id,WxUser wxuser)
        {
            var user = Context.Users.Find(id);
            if (user == null)
            {
                return null;
            }
            user.CopyFrom(wxuser);
            Context.SaveChanges();
            var db = new ZhangSubDb(user.OpenId);
            var subuser = db.Users.Find(user.Id);
            if (subuser==null)
            {
                subuser = user;
                db.Users.Add(subuser);
            }
            subuser.CopyFrom(user);
            db.SaveChanges();
            return user;
        }
        #region 依赖
        /// <summary>
        /// 上下文
        /// </summary>
        public ZhangDb Context { get; set; } = default!;
        #endregion
    }
}