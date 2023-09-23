using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace LQ.Wx.Zhang.WebApi
{
    public class CustomCookieAuthenticationOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public void Configure(string? name, CookieAuthenticationOptions options)
        {
            if (name == "sa")
            {

            }
        }

        public void Configure(CookieAuthenticationOptions options)
        {
            Configure(Options.DefaultName,options);
        }
    }
}
