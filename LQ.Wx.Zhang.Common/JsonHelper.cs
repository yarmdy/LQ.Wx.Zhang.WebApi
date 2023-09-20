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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Html;

namespace LQ.Wx.Zhang.Common
{
    /// <summary>
    /// 字典帮助对象
    /// </summary>
    public static class JsonHelper
    {
        public static JsonSerializerSettings? _setting = null;
        public static JsonSerializerSettings Settings
        {
            get
            {
                if (_setting != null)
                {
                    return _setting;
                }
                var setting = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    //DateFormatString = "yyyy-MM-dd HH:mm:ss",
                };
                setting.Converters.Add(new AutoDateTimeFormat { });
                _setting = setting;
                return _setting;
            }
        }
        public static string SerializeObject(object obj)
        {
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj,Settings);
        }
        public static IHtmlContent ToJson(object obj)
        {
            return new HtmlString(SerializeObject(obj));
        }
    }

    public class AutoDateTimeFormat : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
        private static IsoDateTimeConverter tConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return tConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                tConverter.WriteJson(writer, value, serializer);
                return;
            }
            DateTime dd = (DateTime)value;
            if (dd.Date == dd)
            {
                dConverter.WriteJson(writer, dd, serializer);
                return;
            }
            tConverter.WriteJson(writer, dd, serializer);
        }
    }

}