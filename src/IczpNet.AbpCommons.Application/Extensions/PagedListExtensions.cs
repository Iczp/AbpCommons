using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Linq;
using Volo.Abp.ObjectMapping;

namespace IczpNet.AbpCommons.Extensions;

public static class PagedListExtensions
{
    public static async Task<PagedResultDto<TOuputDto>> ToPagedListAsync<T, TOuputDto>(
        this IQueryable<T> query,
        IAsyncQueryableExecuter asyncExecuter,
        IObjectMapper objectMapper,
        int maxResultCount = 10,
        int skipCount = 0,
        string sorting = null,
        Func<IQueryable<T>, IQueryable<T>> queryableAction = null,
        Func<List<T>, Task<List<T>>> entityAction = null)
    {
        var totalCount = await asyncExecuter.CountAsync(query);

        var items = new List<TOuputDto>();

        if (totalCount > 0 && totalCount > skipCount)
        {
            if (!sorting.IsNullOrWhiteSpace())
            {
                query = query.OrderBy(sorting);
            }
            else if (queryableAction != null)
            {
                query = queryableAction.Invoke(query);
            }

            if (totalCount >= maxResultCount)
            {
                query = ApplyPaging(query, skipCount, maxResultCount);
            }

            var entities = await asyncExecuter.ToListAsync(query);

            if (entityAction != null)
            {
                entities = await entityAction?.Invoke(entities);
            }

            items = objectMapper.Map<List<T>, List<TOuputDto>>(entities);
        }

        return new PagedResultDto<TOuputDto>(totalCount, items);
    }

    private static IQueryable<T> ApplyPaging<T>(IQueryable<T> query, int skipCount, int maxResultCount)
    {
        return query.PageBy(skipCount, maxResultCount);
    }

    public static Task<PagedResultDto<TOuputDto>> ToPagedListAsync<T, TOuputDto>(
        this IQueryable<T> query,
        IAsyncQueryableExecuter asyncExecuter,
        IObjectMapper objectMapper,
        PagedAndSortedResultRequestDto input,
        Func<IQueryable<T>, IQueryable<T>> queryableAction = null,
        Func<List<T>, Task<List<T>>> entityAction = null)
    {
        return ToPagedListAsync<T, TOuputDto>(query, asyncExecuter, objectMapper, input.MaxResultCount, input.SkipCount, input.Sorting, queryableAction, entityAction);
    }
}
