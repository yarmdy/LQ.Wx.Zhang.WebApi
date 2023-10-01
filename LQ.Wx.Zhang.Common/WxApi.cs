using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using System.Text;

namespace LQ.Wx.Zhang.Common
{
    /// <summary>
    /// 加密解密帮助类
    /// </summary>
    public class WxApi
    {
        public const string appid = "wx8861e4b483de04b1";
        public const string secret = "78fea1a733946fb4b35910aad7c0e981";
        public static dynamic Jscode2session(string code)
        {
            var res = new JHttpH().Get($"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={secret}&js_code={code}&grant_type=authorization_code");
            return JDynamicObject.Create(res);
        }
    }
}
