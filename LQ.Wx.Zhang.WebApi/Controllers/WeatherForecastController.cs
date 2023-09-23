using LQ.Wx.Zhang.BLL;
using LQ.Wx.Zhang.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LQ.Wx.Zhang.WebApi.Controllers
{
    [ApiController,Authorize]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public ItemBll ItemBll { get; set; }
        public UserBll UserBll { get; set; }
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger):base()
        {
            _logger = logger;
            ItemBll = Common.HttpContext.Current.RequestServices.GetService<ItemBll>();
            UserBll = Common.HttpContext.Current.RequestServices.GetService<UserBll>();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var user = new User {Name="asd",Account="asd" };
            UserBll.Add(user);
            //var asd = ItemBll.Add(new Item { Name="asd",Code="asd",CreateUser=user});
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Post()
        {
            return Get();
        }
    }
}