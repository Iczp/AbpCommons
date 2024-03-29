﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IczpNet.AbpCommons
{
    public interface ICrudAbpCommonsAppService<
        TGetOutputDto,
        TGetListOutputDto,
        TKey,
        TGetListInput,
        TCreateInput,
        TUpdateInput>
        :
        ICrudAppService<
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput,
            TCreateInput,
            TUpdateInput>
    {
        Task<List<TGetOutputDto>> GetManyAsync(List<TKey> idList);

        Task DeleteManyAsync(List<TKey> idList);
    }

    public interface ICrudAbpCommonsAppService<
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput>
        :
        ICrudAppService<
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {

    }

}
