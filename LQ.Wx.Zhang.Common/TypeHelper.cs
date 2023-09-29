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

namespace LQ.Wx.Zhang.Common
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class TypeHelper
    {
        public const BindingFlags __flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
        public static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.Method;
        }
        public static PropertyInfo[] GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(__flags);
        }
        public static bool HasProperty<T>(string name)
        {
            return GetProperty<T>(name) != null;
        }
        public static bool HasPropertyBase<T>(string name)
        {
            return HasPropertyBase(typeof(T),name);
        }
        public static bool HasPropertyBase(Type type, string name)
        {
            return GetPropertyBase(type,name) != null;
        }
        public static PropertyInfo? GetProperty<T>(string name)
        {
            return GetProperty(typeof(T),name);
        }
        public static PropertyInfo? GetProperty(Type type,string name)
        {
            var arr = name.Split('.');
            PropertyInfo? res = null;
            foreach (var param in arr)
            {
                var prop = GetPropertyBase(type, param);
                if (prop == null)
                {
                    return null;
                }
                type = prop.PropertyType;
                res = prop;
            }
            return res;
        }
        public static PropertyInfo? GetPropertyBase<T>(string name)
        {
            return GetPropertyBase(typeof(T),name);
        }
        public static PropertyInfo? GetPropertyBase(Type type, string name)
        {
            return type.GetProperty(name, __flags);
        }
        public static void SetPropertyValue(this object? obj,string name, object value)
        {
            if (obj == null)
            {
                return;
            }
            var prop = GetPropertyBase(obj.GetType(),name);
            if (prop == null)
            {
                return;
            }
            prop.SetValue(obj, value);
        }
        public static TProperty? GetPropertyValue<TProperty>(this object obj,string name)
        {
            var val = obj.GetPropertyValueObj(name);
            if (!(val is TProperty))
            {
                return default;
            }
            return (TProperty)val;
        }
        public static object? GetPropertyValueObj(this object obj, string name)
        {
            var prop = GetPropertyBase(obj.GetType(), name);
            if (prop == null)
            {
                return default;
            }
            var val = prop.GetValue(obj);
            return val;
        }
    }

}