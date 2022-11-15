using IczpNet.AbpCommons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IczpNet.AbpCommons.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PagedAndFilterRequestDto : PagedRequestDto
    {
        /// <summary>
        /// ¹ýÂË×Ö¶Î
        /// </summary>
        [DefaultValue(null)]
        public virtual List<FilterInput> Filters { get; set; } = new List<FilterInput>();
    }
}