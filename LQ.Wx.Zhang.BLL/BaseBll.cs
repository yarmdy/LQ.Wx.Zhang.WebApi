using LQ.Wx.Zhang.DAL;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LQ.Wx.Zhang.BLL
{
    public class BaseBll<T> where T : BaseEntity
    {
        public BaseBll(ZhangDb db)
        {
            Db = db;
        }

        public ZhangDb Db{get;set;}
    }

    public static class IServiceEx
    {
        public static IServiceCollection AddBll(this IServiceCollection services)
        {
            services.AddScoped<ZhangDb>();
            Assembly.GetExecutingAssembly().GetTypes().Where(a=>a.BaseType!=null && a.BaseType.IsGenericType && a.BaseType.GetGenericTypeDefinition()==typeof(BaseBll<>)).ToList().ForEach(a => {
                services.AddScoped(a);
            });
            return services;
        }
    }
}