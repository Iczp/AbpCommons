using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IczpNet.AbpCommons.Models
{
    /// <summary>
    /// 字段过滤
    /// </summary>
    public class FilterInput
    {
        /// <summary>
        /// 字段Id
        /// </summary>
        public virtual Guid PropertyNameId { get; set; }
        /// <summary>
        /// 运算表达式{
        ///	0: "=",
        ///	1: ">",
        ///	2: ">=",
        ///	3: "&lt;",
        ///	4: "&lt;=",
        ///	5: "!=",
        ///	6: "like"
        ///}
        /// </summary>
        [Description("=,!=,>,>=,<,<=,%,!%")]
        public virtual string ExpressType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        [Required]
        public virtual string Value { get; set; }
        ///// <summary>
        ///// 字段过滤
        ///// </summary>
        //public virtual List<FilterInput> Filters { get; set; }
    }
}
