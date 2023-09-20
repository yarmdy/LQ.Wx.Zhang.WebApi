using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LQ.Wx.Zhang.DAL
{
    /// <summary>
    /// 功能类型
    /// </summary>
    public enum EnumFuncType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Display(Name = "菜单")]
        Menu =0,
        /// <summary>
        /// 按钮
        /// </summary>
        [Display(Name ="按钮")]
        Button=1
    }
    /// <summary>
    /// 可用状态
    /// </summary>
    public enum EnumEnableState
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Display(Name = "启用")]
        Enabled = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Display(Name = "禁用")]
        Disabled = 0
    }
    /// <summary>
    /// 用户类型 1 被测者 2 组织测试者 4 代理商 -2147483648总管理员
    /// </summary>
    public enum EnumUserType
    {
        /// <summary>
        /// 被测者
        /// </summary>
        [Display(Name = "被测者")]
        Tested = 1,
        /// <summary>
        /// 测试者
        /// </summary>
        [Display(Name = "测试者")]
        Testor = 2,
        /// <summary>
        /// 代理商
        /// </summary>
        [Display(Name = "代理商")]
        Agent = 4,
        /// <summary>
        /// 访客
        /// </summary>
        [Display(Name = "访客")]
        Visitor = 8,

        /// <summary>
        /// 管理员
        /// </summary>
        [Display(Name = "管理员")]
        Admin = -2147483648,
    }
    /// <summary>
    /// 附件模块代码 0 未知 100 问题图片 200 
    /// </summary>
    public enum EnumAttachmentModule
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = "未知")]
        None = 0,
        /// <summary>
        /// 问题图片
        /// </summary>
        [Display(Name = "问题图片")]
        Testor = 100,
        /// <summary>
        /// 测试题图片
        /// </summary>
        [Display(Name = "测试题图片")]
        Agent = 200,
    }
    /// <summary>
    /// 附件状态 0创建 1应用 每天状态为0的会被自动清掉以释放空间
    /// </summary>
    public enum EnumAttachmentStatus
    {
        /// <summary>
        /// 创建
        /// </summary>
        [Display(Name = "创建")]
        Create = 0,
        /// <summary>
        /// 应用
        /// </summary>
        [Display(Name = "应用")]
        Apply = 1,
    }

    /// <summary>
    ///  题类型 1单选题 2多选题 3判断题
    /// </summary>
    public enum EnumQuestionType
    {
        /// <summary>
        /// 单选题
        /// </summary>
        [Display(Name = "单选题")]
        Single = 0,
        /// <summary>
        /// 多选题
        /// </summary>
        [Display(Name = "多选题")]
        Multiple = 1,
        /// <summary>
        /// 判断题
        /// </summary>
        [Display(Name = "判断题")]
        Judge = 2,
    }
    /// <summary>
    /// 测试题状态 0 创建中 1 创建完成 2已上架
    /// </summary>
    public enum EnumExampleStatus {
        /// <summary>
        /// 创建中
        /// </summary>
        [Display(Name = "创建中")]
        Creating = 0,
        /// <summary>
        ///创建完成
        /// </summary>
        [Display(Name = "创建完成")]
        Created = 1,
        /// <summary>
        /// 已上架
        /// </summary>
        [Display(Name = "已上架")]
        Listing = 2,
    }
    /// <summary>
    /// 计划状态 0 创建中 1 已创建 2 已发布
    /// </summary>
    public enum EnumPlanStatus
    {
        /// <summary>
        /// 创建中
        /// </summary>
        [Display(Name = "创建中")]
        Creating = 0,
        /// <summary>
        ///创建完成
        /// </summary>
        [Display(Name = "创建完成")]
        Created = 1,
        /// <summary>
        /// 已发布
        /// </summary>
        [Display(Name = "已发布")]
        Published = 2,
    }
    /// <summary>
    /// 学员状态 0 创建 1 进入系统 2 开始答题 3 离开 4 答题完
    /// </summary>
    public enum EnumPlanUserStatus
    {
        /// <summary>
        /// 创建
        /// </summary>
        [Display(Name = "创建")]
        Create = 0,
        /// <summary>
        ///进入系统
        /// </summary>
        [Display(Name = "进入系统")]
        Enter = 1,
        /// <summary>
        /// 开始答题
        /// </summary>
        [Display(Name = "开始答题")]
        Answer = 2,
        /// <summary>
        /// 离开
        /// </summary>
        [Display(Name = "离开")]
        Leave = 3,
        /// <summary>
        /// 答题完
        /// </summary>
        [Display(Name = "答题完")]
        Finish = 4,
    }
    /// <summary>
    /// 学员答题状态 0 未答 1 正在答题 2 答完
    /// </summary>
    public enum EnumPlanUserTestStatus
    {
        /// <summary>
        /// 未答
        /// </summary>
        [Display(Name = "未答")]
        NoAnswer = 0,
        /// <summary>
        ///正在答题
        /// </summary>
        [Display(Name = "正在答题")]
        Answer = 1,
        /// <summary>
        /// 答完
        /// </summary>
        [Display(Name = "答完")]
        Finish = 2,
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
    /// <summary>
    /// 性别
    /// </summary>
    public enum EnumSex
    {
        ///// <summary>
        ///// 保密
        ///// </summary>
        //[Display(Name = "保密")]
        //Normal = 0,
        /// <summary>
        /// 女
        /// </summary>
        [Display(Name = "女")]
        Female = 1,
        /// <summary>
        /// 男
        /// </summary>
        [Display(Name = "男")]
        Male = 2
    }
}
