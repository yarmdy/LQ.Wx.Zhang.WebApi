using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class BaseEntity
    {
        public int CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public int? ModifyUserId { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool IsDel { get; set; }
        public int? DelUserId { get; set; }
        public DateTime? DelTime { get; set; }

        public User CreateUser { get; set; }
        public User? ModifyUser { get; set; }
        public User? DelUser { get; set; }

        [NotMapped]
        public object? ObjectTag { get; set; }
    }
}
