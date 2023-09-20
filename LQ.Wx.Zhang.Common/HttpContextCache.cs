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
using Microsoft.AspNetCore.Http;

namespace AiBi.Test.Common
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

}