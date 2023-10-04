using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.BLL.Model;
using LQ.Wx.Zhang.Common;
using LQ.Wx.Zhang.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LQ.Wx.Zhang.WebApi.Controllers
{
    [ApiController, Authorize]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public CoreBll Bll { get; set; }
        public UserBll UserBll { get; set; }
        public AccountController() { 
            Bll = Common.HttpContext.Current!.RequestServices.GetRequiredService<CoreBll>();
            UserBll = Common.HttpContext.Current!.RequestServices.GetRequiredService<UserBll>();
        }
        [HttpGet("Login"),AllowAnonymous]
        public object Login()
        {
            var res = new Response();
            res.code = EnumResStatus.NoLogin; return res;
        }
        [HttpPost("Login"), AllowAnonymous]
        public object Login2()
        {

            return Login();
        }
        [HttpPost("GetCookie"), AllowAnonymous]
        public async Task<object> Login3(string code)
        {
            var res = new Response<User,object>();
            var httpres = WxApi.Jscode2session(code);
            var openId = (string)httpres.openid;
            var sessionKey = (string)httpres.session_key;
            if (string.IsNullOrEmpty(openId)) {
                res.code = EnumResStatus.Fail;
                return res;
            }
            var user = Bll.GetUser(openId,sessionKey);
            if (user == null)
            {
                res.code = EnumResStatus.NoPermissions;
                return res;
            }
            string cookiestr = await setCookie(user);
            res.data = user;
            res.data2 = new { cookie=cookiestr};
            return res;
        }
        [HttpPost("CreateUser"), AllowAnonymous]
        public async Task<object> Login4(string code,WxUserInfo model)
        {
            var res = new Response<User,object>();
            if (model == null || model.UserInfo == null)
            {
                res.code = EnumResStatus.Fail;
                return res;
            }
            var httpres = WxApi.Jscode2session(code);
            var openId = (string)httpres.openid;
            var sessionKey = (string)httpres.session_key;
            if (string.IsNullOrEmpty(openId))
            {
                res.code = EnumResStatus.Fail;
                return res;
            }
            var user = Bll.GetUser(openId, sessionKey, true);
            if (user == null)
            {
                res.code = EnumResStatus.NoPermissions;
                return res;
            }
            user = Bll.SetUserInfo(user.Id, model.UserInfo)!;
            string cookiestr = await setCookie(user);
            res.data = user;
            res.data2 = new { cookie = cookiestr };
            return res;
        }

        private async Task<string> setCookie(User user)
        {
            var claims = new List<Claim>() {
                new Claim("UserName", $"{user.Id}|{user.OpenId}|{user.NickName}"),
                new Claim("Role", "user"),
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie", "UserName", "Role")));

            var cookie = Response.Headers["Set-Cookie"];
            var cookiestr = new System.Text.RegularExpressions.Regex(@"(sa=.+?)\;").Match(cookie!).Groups[1].Value;
            return cookiestr;
        }

        [HttpPost("SetUserInfo"),Authorize]
        public object SetUserInfo(WxUserInfo model) {
            var res = new Response();
            var userId = UserBll.CurrentUserId;
            if (model.UserInfo == null)
            {
                res.code=EnumResStatus.Fail;
                return res;
            }
            var user = Bll.SetUserInfo(userId,model.UserInfo);
            res.data = user;
            return res;
        }
    }
}