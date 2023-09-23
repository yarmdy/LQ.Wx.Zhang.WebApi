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
    }
}
