using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class ItemClass : IdEntity
    {
        public ItemClass()
        {
            Items = new HashSet<Item>();
        }
        public string Name { get; set; } = "";
        public virtual ICollection<Item> Items { get; set; }
    }
}
