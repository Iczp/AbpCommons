using System.ComponentModel;

namespace IczpNet.AbpCommons.Enums
{
    /// <summary>
    /// 运算表达式{
    ///	0: "=",
    ///	1: "!=",
    ///	2: ">",
    ///	3: ">=",
    ///	4: "&lt;",
    ///	5: "&lt;=",
    ///	6: "like"
    ///	7: "unlike"
    ///}
    /// </summary>
    public enum ExpressTypeEnum
    {
        /// <summary>
        /// 等于 =
        /// </summary>
        [Description("=")]
        Equal = 0,
        /// <summary>
        /// 不等于 !=
        /// </summary>
        [Description("!=")]
        NotEqual = 1,
        /// <summary>
        /// 大于 >
        /// </summary>
        [Description(">")]
        GreaterThan = 2,
        /// <summary>
        /// 大于或等于 >=
        /// </summary>
        [Description(">=")]
        GreaterThanOrEqual = 3,
        /// <summary>
        /// 小于 &lt;
        /// </summary>
        [Description("<")]
        LessThan = 4,
        /// <summary>
        /// 小于或等于 &lt;=
        /// </summary>
        [Description("<=")]
        LessThanOrEqual = 5,
        /// <summary>
        /// Like
        /// </summary>
        [Description("%")]
        Like = 6,
        /// <summary>
        /// Like
        /// </summary>
        [Description("!%")]
        Unlike = 7,
    }
}
