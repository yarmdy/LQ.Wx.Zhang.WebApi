using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class BeginData
    {
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
        public decimal Number { get; set; }
        public decimal Money { get; set; }

        public virtual StockDetail? StockDetail { get; set; }
    }
}
