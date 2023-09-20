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

namespace AiBi.Test.Common
{
    /// <summary>
    /// 字典帮助对象
    /// </summary>
    public static class DicHelper 
    {
        /// <summary>
        /// 不至于报错的字典取值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue? G<TKey,TValue>(this IDictionary<TKey,TValue> dic,TKey key,TValue? def = default) { 
            return key!=null && dic.ContainsKey(key) ? dic[key]:def;
        }

        public static TValue? G<TValue>(this IDictionary dic, object key, TValue? def = default) {
            if (key==null || !dic.Contains(key))
            {
                return def;
            }
            var val = dic[key];
            if(val is TValue)
            {
                return (TValue)val;
            }
            return def;
        }

        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dic,Action<KeyValuePair<TKey, TValue>> func)
        {
            dic.ToList().ForEach(func);
        }
    }

}