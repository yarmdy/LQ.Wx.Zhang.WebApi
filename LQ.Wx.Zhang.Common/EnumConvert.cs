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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace AiBi.Test.Common
{
    /// <summary>
    /// 枚举转换对象
    /// </summary>
    public static class EnumConvert
    {
        public static List<EnumObject> ToList<T>(T? selectedValue = null) where T : struct,Enum
        {
            var type = typeof(T);
            var fields = type.GetFields();
            var values = Enum.GetValues(typeof(T));
            var list = new List<EnumObject>();
            foreach (T val in values)
            {
                var obj = new EnumObject { };
                var name = val + "";
                var field = fields.FirstOrDefault(a => a.Name == name);
                if (field == null)
                {
                    goto loopend;
                }
                
                
                obj.Name = name;
                obj.Value = (int)(object)val;
                obj.Selected = name+""==selectedValue+"";
                var desctmp = field.GetCustomAttribute(typeof(DisplayAttribute));
                var desc = desctmp == null ? null : (desctmp as DisplayAttribute);

                obj.Text = desc?.Name ?? name;
                obj.Order = desc?.GetOrder() ?? 0;
                obj.Desc = desc?.Description ?? name;
            loopend:
                list.Add(obj);
            }
            return list; ;
        } 
    }

    public class EnumObject
    {
        public string? Name { get; set; }
        public object? Value { get; set; }
        public string? Text { get; set; }

        public bool Selected { get; set; }

        public int Order { get; set; }

        public string? Desc { get; set; }
    }
}