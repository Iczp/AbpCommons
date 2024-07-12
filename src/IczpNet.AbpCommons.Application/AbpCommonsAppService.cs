using IczpNet.AbpCommons.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using IczpNet.AbpCommons.Extensions;

namespace IczpNet.AbpCommons;

public abstract class AbpCommonsAppService : ApplicationService
{
    protected AbpCommonsAppService()
    {
        LocalizationResource = typeof(AbpCommonsResource);
        ObjectMapperContext = typeof(AbpCommonsApplicationModule);
    }

    protected virtual async Task<PagedResultDto<TOuputDto>> GetPagedListAsync<T, TOuputDto>(
        IQueryable<T> query,
        PagedAndSortedResultRequestDto input,
        Func<IQueryable<T>, IQueryable<T>> queryableAction = null,
        Func<List<T>, Task<List<T>>> entityAction = null)
    {
        return await query.ToPagedListAsync<T, TOuputDto>(AsyncExecuter, ObjectMapper, input, queryableAction, entityAction);
    }

    protected virtual async Task<PagedResultDto<T>> GetPagedListAsync<T>(
        IQueryable<T> query,
        PagedAndSortedResultRequestDto input,
        Func<IQueryable<T>, IQueryable<T>> queryableAction = null,
        Func<List<T>, Task<List<T>>> entityAction = null)
    {
        return await GetPagedListAsync<T, T>(query, input, queryableAction, entityAction);
    }

}
