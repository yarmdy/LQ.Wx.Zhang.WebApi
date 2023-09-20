using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LQ.Wx.Zhang.BLL
{
    public enum EnumResStatus
    {
        Succ=1,
        Fail = -1,
        NoPermissions=-2,
        NoLogin=-3
    }
    public class Response
    {
        private EnumResStatus _code=EnumResStatus.Succ;
        public EnumResStatus code
        {
            get
            {
                return _code;
            }
            set {
                if (_code == value)
                {
                    return;
                }
                _code = value;
                if(msg!= "操作成功" && msg != "操作失败" && msg != "没有权限" && msg != "没有登录")
                {
                    return;
                }
                switch (value)
                {
                    case EnumResStatus.Succ: {
                            msg = "操作成功";
                        }break; case EnumResStatus.Fail: {
                            msg = "操作失败";
                        }break; case EnumResStatus.NoPermissions: {
                            msg = "没有权限";
                        }break;case EnumResStatus.NoLogin: {
                            msg = "没有登录";
                        }break;
                }
                
            }
        }
        public string msg { get; set; } = "操作成功";

        public object? data { get; set; }

        public int count { get; set; }
    }

    public class Response<T>:Response { 
        new public T? data { get; set; }
    }
    public class Response<T,T2> : Response<T>
    {
        public T2? data2 { get; set; }
    }
    public class Response<T,T2,T3> : Response<T,T2>
    {
        public T3? data3 { get; set; }
    }
    public class Response<T,T2,T3,T4> : Response<T,T2,T3>
    {
        public T4? data4 { get; set; }
    }
    
}