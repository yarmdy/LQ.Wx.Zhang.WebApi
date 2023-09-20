using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace LQ.Wx.Zhang.Common
{
    /// <summary>
    /// http上下文缓存
    /// </summary>
    public class HttpContextCache
    {
        public static bool CanUse { get { return HttpContext.Current != null; } }
        public static HttpContextCache Cache { get; }=new HttpContextCache();
        /// <summary>
        /// http上下文物品
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public object? this[object key]
        {
            get
            {
                var context = HttpContext.Current;
                if(context==null) return null;
                return context.Items[key];
            }
            set
            {
                var context = HttpContext.Current;
                if (context == null) return;
                context.Items[key] = value;
            }
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue? G<TValue>(object key)
        {
            var context = HttpContext.Current;
            if (context == null) return default;
            return context.Items.G<TValue>(key);
        }
    }

    public static class HttpContext
    {
        public static IServiceProvider? ServiceProvider { get; set; }
        public static Microsoft.AspNetCore.Http.HttpContext? Current { get
            {
                if (ServiceProvider == null)
                {
                    return null;
                }
                return ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            } 
        }
        public static IServiceCollection AddHttpContextHelper(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
        public static T GetService<T>() 
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException();
            }
            var res = ServiceProvider.GetService(typeof(T));
            if (res == null)
            {
                throw new InvalidOperationException();
            }
            return (T)res;
        }
    }
}