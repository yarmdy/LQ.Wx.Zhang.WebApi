using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class Item : IdEntity
    {
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int? ImageId { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public virtual Attachment? Image { get; set; }
        public int ClassId { get; set; }
        public virtual ItemClass? ItemClass { get; set; }
    }
}
