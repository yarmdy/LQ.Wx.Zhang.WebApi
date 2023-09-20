using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

using System.Text;

namespace AiBi.Test.Common
{
    /// <summary>
    /// 异常
    /// </summary>
    public static class ExceptionEx
    {
        public static string? GetInnerMessage(this Exception ex)
        {
            var list = new List<Exception?> { ex.InnerException};
            var res = ex.Message;
            do {
                var first = list.LastOrDefault();
                if (first == null)
                {
                    return res;
                }
                list.RemoveAt(list.Count-1);
                list.Add(first.InnerException);
                res = first.Message;
            }while (list.Count>0);
            return null;
        }
    }
}
