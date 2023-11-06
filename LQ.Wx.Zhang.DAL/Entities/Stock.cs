using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class Stock : IdEntity
    {
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
        public decimal Number { get; set; }
        public decimal Money { get; set; }
    }
    public class StockDetail : IdEntity
    {
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
        public decimal Number { get; set; }
        public decimal Money { get; set; }

        public decimal BalNumber { get; set; }
        public decimal BalMoney { get; set; }
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
        public int? OrderId { get; set; }
        public int? OrderDetailId { get; set; }
        public int? BeginId { get; set; }
        public virtual InOutOrder? Order { get; set; }
        public virtual InOutOrderDetail? OrderDetail { get; set; }
        public virtual BeginData? BeginData { get; set; }
    }
}
