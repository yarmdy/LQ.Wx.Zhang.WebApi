using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class Client:IdEntity
    {
        public Client() {
            Orders = new HashSet<InOutOrder>();
        }
        public string Name { get; set; } = "";
        public virtual ICollection<InOutOrder> Orders { get; set; }
    }
}
