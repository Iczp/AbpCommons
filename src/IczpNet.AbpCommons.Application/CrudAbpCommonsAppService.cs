using IczpNet.AbpCommons.DataFilters;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.AbpCommons
{

    public abstract class CrudAbpCommonsAppService<
        TEntity,
        TGetOutputDto,
        TGetListOutputDto,
        TKey,
        TGetListInput,
        TCreateInput,
        TUpdateInput>
        :
        CrudAppService<
            TEntity,
            TGetOutputDto,
            TGetListOutputDto,
            TKey,
            TGetListInput,
            TCreateInput,
            TUpdateInput>
         ,
    ICrudAbpCommonsAppService<
        TGetOutputDto,
        TGetListOutputDto,
        TKey,
        TGetListInput,
        TCreateInput,
        TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        protected CrudAbpCommonsAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {

        }

        //[HttpGet]
        public override async Task<TGetOutputDto> GetAsync(TKey id)
        {
            await CheckGetPolicyAsync(id);

            var entity = await GetEntityByIdAsync(id);

            return await MapToGetOutputDtoAsync(entity);
        }

        protected virtual Task CheckGetPolicyAsync(TKey id)
        {
            return CheckGetPolicyAsync();
        }

        //[HttpGet]
        public virtual async Task<List<TGetOutputDto>> GetManyAsync(List<TKey> idList)
        {
            var list = new List<TGetOutputDto>();

            foreach (var id in idList)
            {
                list.Add(await base.GetAsync(id));
            }
            return list;
        }

        //[HttpGet]
        public override async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            await CheckGetListPolicyAsync(input);

            var query = await CreateFilteredQueryAsync(input);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var entityDtos = new List<TGetListOutputDto>();

            if (totalCount > 0)
            {
                query = ApplySorting(query, input);

                query = ApplyPaging(query, input);

                var entities = await AsyncExecuter.ToListAsync(query);

                entityDtos = await MapToGetListOutputDtosAsync(entities);
            }

            return new PagedResultDto<TGetListOutputDto>(totalCount, entityDtos);
        }

        protected virtual Task CheckGetListPolicyAsync(TGetListInput input)
        {
            return CheckGetListPolicyAsync();
        }

        //[HttpPost]
        public override async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            await CheckCreatePolicyAsync(input);

            await CheckCreateAsync(input);

            var entity = await MapToEntityAsync(input);

            await SetCreateEntityAsync(entity, input);

            TryToSetTenantId(entity);

            await Repository.InsertAsync(entity, autoSave: true);

            //await base.CreateAsync(input);

            return await MapToGetOutputDtoAsync(entity);
        }

        protected virtual Task CheckCreatePolicyAsync(TCreateInput input)
        {
            return CheckCreatePolicyAsync();
        }

        protected virtual Task CheckCreateAsync(TCreateInput input)
        {
            return Task.CompletedTask;
        }

        protected virtual Task SetCreateEntityAsync(TEntity entity, TCreateInput input)
        {
            return Task.CompletedTask;
        }

        //[HttpPost]
        public override async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            await CheckUpdatePolicyAsync(id, input);

            var entity = await GetEntityByIdAsync(id);

            await CheckUpdateAsync(id, entity, input);

            //TODO: Check if input has id different than given id and normalize if it's default value, throw ex otherwise
            await MapToEntityAsync(input, entity);

            await SetUpdateEntityAsync(entity, input);

            await Repository.UpdateAsync(entity, autoSave: true);

            return await MapToGetOutputDtoAsync(entity);
        }

        protected virtual Task CheckUpdatePolicyAsync(TKey id, TUpdateInput input)
        {
            return CheckUpdatePolicyAsync();
        }

        protected virtual Task CheckUpdateAsync(TKey id, TEntity entity, TUpdateInput input)
        {
            return Task.CompletedTask;
        }

        protected virtual Task SetUpdateEntityAsync(TEntity entity, TUpdateInput input)
        {
            return Task.CompletedTask;
        }

        //[HttpPost]
        public override async Task DeleteAsync(TKey id)
        {
            await CheckDeletePolicyAsync(id);

            var entity = await GetEntityByIdAsync(id);

            await CheckDeleteAsync(entity);

            await DeleteByIdAsync(id);
        }

        protected virtual Task CheckDeletePolicyAsync(TKey id)
        {
            return CheckDeletePolicyAsync();
        }

        protected virtual async Task CheckDeleteAsync(TEntity entity)
        {
            await CheckDeleteIsStaticAsync(entity);
        }

        protected virtual Task CheckDeleteIsStaticAsync(TEntity entity)
        {
            var propInfo = entity.GetType().GetProperty(nameof(IIsStatic.IsStatic));

            Assert.If(entity is IIsStatic && propInfo != null && (bool)propInfo.GetValue(entity), "IsStatic=True,cannot delete.");

            return Task.CompletedTask;
        }

        //[HttpPost]
        public virtual async Task DeleteManyAsync(List<TKey> idList)
        {
            foreach (var id in idList)
            {
                await DeleteAsync(id);
            }
        }
    }
}
