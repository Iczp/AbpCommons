using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.AbpCommons
{
    public abstract class CrudCommonAppService<
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
    ICrudCommonAppService<
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
        protected CrudCommonAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {

        }

        [HttpGet]
        public override Task<TGetOutputDto> GetAsync(TKey id)
        {
            return base.GetAsync(id);
        }

        [HttpGet]
        public virtual async Task<List<TGetOutputDto>> GetManyAsync(List<TKey> idList)
        {
            var list = new List<TGetOutputDto>();

            foreach (var id in idList)
            {
                list.Add(await base.GetAsync(id));
            }
            return list;
        }

        [HttpGet]
        public override Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            return base.GetListAsync(input);
        }

        [HttpPost]
        public override Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            return base.CreateAsync(input);
        }

        [HttpPost]
        public override Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            return base.UpdateAsync(id, input);
        }

        [HttpPost]
        public override Task DeleteAsync(TKey id)
        {
            return base.DeleteAsync(id);
        }

        [HttpPost]
        public virtual async Task DeleteManyAsync(List<TKey> idList)
        {
            foreach(var id in idList)
            {
                await base.DeleteAsync(id);
            }
        }
    }
}
