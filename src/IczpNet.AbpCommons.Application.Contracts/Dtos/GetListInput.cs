using IczpNet.AbpCommons.DataFilters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace IczpNet.AbpCommons.Dtos;

public class GetListInput : PagedAndSortedResultRequestDto, IKeyword
{
    /// <summary>
    /// 关键字(支持拼音)
    /// </summary>
    [DefaultValue(null)]
    public virtual string Keyword { get; set; }

    /// <summary>
    /// 显示数量
    /// </summary>
    //[DefaultValue(null)]
    [Range(1, 1000)]
    public override int MaxResultCount { get; set; } = 10;

    /// <summary>
    /// 跳过数量
    /// </summary>
    //[DefaultValue(0)]
    public override int SkipCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [DefaultValue(null)]
    public override string Sorting { get; set; }
}
