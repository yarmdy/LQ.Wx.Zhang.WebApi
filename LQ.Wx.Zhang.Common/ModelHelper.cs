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
using System.Linq.Expressions;
using System.Collections;
using Microsoft.AspNetCore.Html;

namespace AiBi.Test.Common
{
    public static class ModelHelper
    {
        /// <summary>
        /// 拷贝属性
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static T1 CopyFrom<T1, T2>(this T1 obj, T2 obj2) where T1 : class where T2 : class
        {
            return obj.CopyFromExcept(obj2, null);
        }
        public static T1 CopyFrom<T1, T2, T3>(this T1 obj, T2 obj2, Expression<Func<T1, T3>>? except, IEnumerable<Type>? types=null) where T1 : class where T2 : class
        {
            string[]? exceptArr = null;
            if (except == null)
            {
                goto finish;
            }
            if (except.Body.NodeType != ExpressionType.New && except.Body.NodeType != ExpressionType.MemberAccess)
            {
                goto finish;
            }
            if (except.Body.NodeType == ExpressionType.MemberAccess)
            {
                exceptArr = new[] { ((MemberExpression)except.Body).Member.Name };
                goto finish;
            }
            exceptArr = ((NewExpression)except.Body).Arguments.Where(a => a.NodeType == ExpressionType.MemberAccess).Select(a => ((MemberExpression)a).Member.Name).ToArray();

        finish:
            return obj.CopyFromExcept(obj2, exceptArr,types);
        }
        /// <summary>
        /// 拷贝属性
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static T1 CopyFromExcept<T1, T2>(this T1 obj, T2? obj2, string[]? except, IEnumerable<Type>? types = null) where T1 : class where T2 : class
        {
            if (obj2 == null)
            {
                return obj;
            }
            var t2props = typeof(T2).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).ToDictionary(a => a.Name.ToLower());
            if (t2props.Count <= 0)
            {
                return obj;
            }
            var exceptDic = except == null ? new Dictionary<string, bool>() : except.Select(a => (a + "").ToLower()).Distinct().ToDictionary(a => a, a => false);
            types = types?? Enumerable.Empty<Type>();

            var t1props = typeof(T1).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance);
            foreach (var prop in t1props)
            {
                try
                {
                    var name = prop.Name.ToLower();
                    if (exceptDic.ContainsKey(name))
                    {
                        continue;
                    }
                    if (types.Any(a =>
                    {
                        if (a == prop.PropertyType)
                        {
                            return true;
                        }
                        if (a.IsAssignableFrom(prop.PropertyType))
                        {
                            return true;
                        }
                        if (!a.IsGenericType || !prop.PropertyType.IsGenericType)
                        {
                            return false;
                        }
                        if (a.GetGenericTypeDefinition() == prop.PropertyType.GetGenericTypeDefinition())
                        {
                            return true;
                        }

                        return false;
                    })) {
                        continue;
                    }
                    var pf = obj.GetType().GetField($"<{prop.Name}>i__Field", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (!t2props.ContainsKey(name))
                    {
                        continue;
                    }
                    var prop2 = t2props[name];
                    var p1type = prop.PropertyType;
                    var p2type = prop2.PropertyType;

                    var p2v = prop2.GetValue(obj2);
                    if (p1type == p2type)
                    {
                        //prop.SetValue(obj, p2v);
                        if (prop.SetMethod == null && pf != null)
                        {
                            pf.SetValue(obj, p2v);
                        }
                        else if (prop.SetMethod != null)
                        {
                            prop.SetValue(obj, p2v);
                        }
                        continue;
                    }
                    bool p1null = false;
                    bool p2null = false;
                    if ((p1type.FullName??"").StartsWith("System.Nullable") && p1type.GenericTypeArguments != null && p1type.GenericTypeArguments.Length > 0)
                    {
                        p1type = p1type.GenericTypeArguments[0];
                        p1null = true;
                    }
                    if ((p2type.FullName??"").StartsWith("System.Nullable") && p2type.GenericTypeArguments != null && p2type.GenericTypeArguments.Length > 0)
                    {
                        p2type = p2type.GenericTypeArguments[0];
                        p2null = true;
                    }
                    if (p1type != p2type)
                    {
                        continue;
                    }
                    if (!p1null && p2null && p2v == null)
                    {
                        p2v = Activator.CreateInstance(p2type);
                    }
                    //prop.SetValue(obj, p2v);
                    if (prop.SetMethod == null && pf != null)
                    {
                        pf.SetValue(obj, p2v);
                    }
                    else if (prop.SetMethod != null)
                    {
                        prop.SetValue(obj, p2v);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            return obj;
        }
        /// <summary>
        /// 合并实体未dic
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="obj"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static Dictionary<string, object?> CombineToDic<T1, T2>(this T1 obj, T2? obj2)
        {
            var t1 = typeof(T1);
            var t2 = typeof(T2);
            var props1 = t1.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).Select(a => new KeyValuePair<string, object?>(a.Name, a.GetValue(obj))).ToDictionary(a => a.Key, a => a.Value);
            var props2 = t2.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).Select(a => new KeyValuePair<string, object?>(a.Name, obj2 == null ? DefaultForType(a.PropertyType) : a.GetValue(obj2))).ToDictionary(a => a.Key, a => a.Value);
            foreach(var dic in props2)
            {
                var a = dic.Key;
                var b = dic.Value;
                if (props1.ContainsKey(a))
                {
                    props1[a + "_2"] = b;
                }
                else
                {
                    props1[a] = b;
                }
            }
            

            return props1;
        }
        public static Dictionary<string,object?> Obj2Dic<T1>(this T1 obj)
        {
            return typeof(T1).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).Select(a => new KeyValuePair<string, object?>(a.Name, a.GetValue(obj))).ToDictionary(a => a.Key, a => a.Value);
        }
        /// <summary>
        /// 获取实体的json形式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHtmlContent TypeModel<T>()
        {
            var t = typeof(T);
            return new HtmlString(Newtonsoft.Json.JsonConvert.SerializeObject(t.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).ToDictionary(a => a.Name, a => "")));
        }

        public static T? CopyFromDict<T>(Dictionary<string, object> dict) where T : class
        {
            var result = Activator.CreateInstance(typeof(T)) as T;
            if (dict == null)
            {
                return result;
            }


            var t1props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance);
            foreach (var prop in t1props)
            {
                try
                {
                    var name = prop.Name;
                    if (!dict.ContainsKey(name))
                    {
                        continue;
                    }
                    prop.SetValue(result, dict[name]);

                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            return result;
        }

        public static object? DefaultForType<T>()
        {
            return typeof(T).IsValueType ? Activator.CreateInstance(typeof(T)) : null;
        }
        public static object? DefaultForType(Type type)
        {
            return DefaultForType<Type>();
        }
    }
}