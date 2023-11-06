using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class InOutOrder:IdEntity
    {
        public InOutOrder() {
            Details = new HashSet<InOutOrderDetail>();
        }
        public string OrderNo { get; set; } = "";
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }

        public int InOutType { get; set; }
        [NotMapped]
        public InOutType inOutTypeValue
        {
            get
            {
                return (InOutType)InOutType;
            }
            set
            {
                InOutType = (int)value;
            }
        }
        public virtual ICollection<InOutOrderDetail> Details { get; set; }
    }
    public class InOutOrderDetail:IdEntity
    {
        public int OrderId { get; set; }
        public virtual InOutOrder? inOutOrder { get; set; }
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
        public decimal Number { get; set; }
        public decimal Money { get; set; }

        public virtual StockDetail? StockDetail { get; set; }

    }
}
