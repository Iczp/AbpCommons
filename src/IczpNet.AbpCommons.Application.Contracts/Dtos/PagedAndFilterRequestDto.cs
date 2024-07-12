using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace IczpNet.AbpCommons.Dtos;

/// <summary>
/// 
/// </summary>
[Serializable]
public class PagedRequestDto : PagedAndSortedResultRequestDto
{

    [DefaultValue(null)]
    public override string Sorting { get; set; }

    [DefaultValue(10)]
    public override int MaxResultCount { get; set; }

    [DefaultValue(0)]
    public override int SkipCount { get; set; }
}