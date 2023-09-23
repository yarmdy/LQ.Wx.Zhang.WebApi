using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class Attachment : IdEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Ext { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int ModuleType { get; set; }
        public virtual ICollection<Item>? Items { get; set; }
        [NotMapped]
        public string FullName => Name + Path + Ext;
    }
}
