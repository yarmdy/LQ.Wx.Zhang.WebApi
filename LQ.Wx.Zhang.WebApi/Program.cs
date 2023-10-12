

using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.Common;
using LQ.Wx.Zhang.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddBll();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextHelper();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"d:\sa.key"))
//    .SetApplicationName("sa");

builder.Services.AddSingleton<IDataProtectionProvider, MyDataProtectionProvider>();
builder.Services.AddAuthentication("sa").AddCookie("sa"
    , option => {
        builder.Configuration.Bind("CookieSettings", option);
        option.Cookie.Name = "sa";
        option.Cookie.Path = "/";
        //option.Cookie.Domain = "localhost";
        option.Cookie.HttpOnly = true;
        option.Cookie.SameSite = SameSiteMode.None;
    });
//builder.Services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, CustomCookieAuthenticationOptions>();
//builder.Services.AddScoped<IAuthenticationHandlerProvider, MyAuthenticationHandlerProvider>();
//builder.Services.AddScoped<IAuthenticationService, MyAuthenticationService>();

//builder.Services.AddSingleton<ISecureDataFormat<AuthenticationTicket>, MyTicketDataFormat>();

var app = builder.Build();
LQ.Wx.Zhang.Common.HttpContext.ServiceProvider = app.Services;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) => { 
    await next(context);
});
app.UseExceptionHandler(a => { 
    
});
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
