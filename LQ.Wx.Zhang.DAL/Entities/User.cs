using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class User : IdEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;

        public string OpenId { get; set; }=string.Empty;
        public string SessionKey { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Gender { get; set; }
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public new int? CreateUserId { get; set; }
    }
}
