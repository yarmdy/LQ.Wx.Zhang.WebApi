using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LQ.Wx.Zhang.WebApi.Controllers
{
    [ApiController,Authorize]
    [Route("[controller]")]
    public class AccountController : BaseController<User,UserReq.Page>
    {

        public override BaseBll<User, UserReq.Page> Bll => UserBll;

        [AllowAnonymous]
        [HttpGet("Login")]
        public object Login()
        {
            var res = new Response();
            res.code = EnumResStatus.NoLogin; return res;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public object Login2()
        {

            return Login();
        }
        [AllowAnonymous]
        [HttpPost("GetCookie")]
        public async Task<object> Login3(string userName, string password)
        {
            var res = new Response<string>();

            var claims = new List<Claim>() {
                new Claim("UserName", userName),
                new Claim("Password", password)
            };
            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie", "UserName", "role")));

            var cookie = Response.Headers["Set-Cookie"];
            var cookiestr = new System.Text.RegularExpressions.Regex(@"sa=(.+?)\;").Match(cookie!).Groups[1].Value;
            res.data = cookiestr;
            return res;
        }
    }
}