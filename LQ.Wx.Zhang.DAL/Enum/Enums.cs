using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LQ.Wx.Zhang.DAL
{
    public enum InOutType : int
    {
        [Display(Name = "入库")]
        In = 1,
        [Display(Name = "出库")]
        Out = 2,
        [Display(Name = "期初")]
        BeginData = 3
    }
    /// <summary>
    /// 筛选isdel的模式
    /// </summary>
    public enum EnumDeleteFilterMode
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Display(Name ="正常的")]
        Normal=1,
        /// <summary>
        /// 删除
        /// </summary>
        [Display(Name = "删除的")]
        Deleted =2,
        /// <summary>
        /// 全部
        /// </summary>
        [Display(Name = "全部的")]
        All =3
    }
}
