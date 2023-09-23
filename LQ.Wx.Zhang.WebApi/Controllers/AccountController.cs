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
    public class AccountController : ControllerBase
    {
        public ItemBll ItemBll { get; set; }
        public UserBll UserBll { get; set; }

        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger):base()
        {
            _logger = logger;
            ItemBll = Common.HttpContext.Current.RequestServices.GetService<ItemBll>();
            UserBll = Common.HttpContext.Current.RequestServices.GetService<UserBll>();
        }
        [AllowAnonymous]
        [HttpGet("Login")]
        public object Login()
        {
            return new{code=0,msg="ÄúÃ»ÓÐµÇÂ¼" };
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public object Login2()
        {

            return Login();
        }
        [AllowAnonymous]
        [HttpPost("Login2")]
        public async Task<object> Login3(string userName, string password)
        {
            var claims = new List<Claim>() {
                new Claim("UserName", userName),
                new Claim("Password", password)
            };
            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie", "user", "role")));
            return new { userName, password };
        }
    }
}