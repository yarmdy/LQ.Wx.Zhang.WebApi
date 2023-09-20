using System;
using System.Collections.Generic;

namespace LQ.Wx.Zhang.BLL
{
    public class BaseReq 
    {
        
    }
    public class PageReq:BaseReq
    {
        public int Page { get; set; }
        public int Size { get; set; }

        public string? keyword { get; set; }

        public Dictionary<string, string>? Where { get; set; }
        public Dictionary<string,bool>? Sort { get; set; }

        public object? Tag { get; set; }
    }
    public class EditPartsReq : BaseReq
    {
        public Dictionary<string, string>? Properties { get; set; }
    }
}
