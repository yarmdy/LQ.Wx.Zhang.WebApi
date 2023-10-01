using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.BLL.Model
{
    public class WxUser
    {
        public string? NickName { get; set; }
        public string? AvatarUrl { get; set; }
        public int? Gender { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
    public class WxUserInfo
    {
        public WxUser? UserInfo { get; set; }
        public string? RawData { get; set; }
        public string? Signature { get; set; }
        public string? EncryptedData { get; set; }
        public string? Iv { get; set; }
        public string? CloudID { get; set; }
    }
}
