using IczpNet.AbpCommons.DataFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// ResetListExtension
/// </summary>
public static class ResetListExtension
{
    /// <summary>
    ///  重新设置列表的值（删除、修改、新增）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TInput">输入类型</typeparam>
    /// <param name="entityList">实体列表</param>
    /// <param name="inputList">输入列表</param>
    /// <param name="deleteFunction">删除功能</param>
    /// <param name="repository">仓储</param>
    /// <param name="modifyAction">修改功能</param>
    /// <param name="createFunction">新增功能</param>
    /// <returns></returns>
    public static void Reset<TEntity, TInput>(this IList<TEntity> entityList, IList<TInput> inputList, Func<TEntity, TInput, TEntity> createFunction, Func<TEntity, TInput, TEntity> modifyAction = null, Func<List<Guid>, bool> deleteFunction = null, IRepository<TEntity, Guid> repository = null)
        where TEntity : class, IEntity<Guid>, new()
        where TInput : class, IHasId<Guid?>
    {
        //===============
        if (inputList == null || inputList.Count == 0)
        {
            return;
            //throw new AbpException($"输入列表不能为Null");
        }
        var inputIdList = inputList.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();
        var allIdList = entityList.Select(x => x.Id).ToList();

        //删除的
        if (deleteFunction != null)
        {
            var deleteldIdList = allIdList.Except(inputIdList).ToList();
            if (deleteldIdList.Count > 0)
            {
                if (deleteFunction == null || deleteFunction.Invoke(deleteldIdList))
                {
                    foreach (var deleteId in deleteldIdList)
                    {
                        var item = entityList.Single(x => x.Id == deleteId);
                        var index = entityList.IndexOf(item);
                        entityList.RemoveAt(index);
                    }
                    if (repository != null)
                    {
                        //repository.Delete(x => deleteldIdList.Contains(x.Id));
                    }
                }
            }
        }

        //修改的
        if (modifyAction != null)
        {
            var modifyIdList = inputIdList.Intersect(allIdList).ToList();
            var modifyEntityList = entityList.Where(x => modifyIdList.Contains(x.Id)).ToList();
            foreach (var entity in modifyEntityList)
            {
                var inputField = inputList.FirstOrDefault(d => d.Id == entity.Id);
                if (inputField != null)
                {
                    modifyAction(entity, inputField);
                }
            }
        }
        //新增的
        if (createFunction != null)
        {
            var newFieldList = inputList.Where(x => !x.Id.HasValue).ToList();
            foreach (var inputField in newFieldList)
            {
                var newEntity = createFunction(new TEntity(), inputField);
                entityList.Add(newEntity);
            }
        }
    }
    /// <summary>
    ///  重新设置列表的值（删除、修改、新增）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TInput">输入类型</typeparam>
    /// <param name="entityList">实体列表</param>
    /// <param name="inputList">输入列表</param>
    /// <param name="deleteAsync">删除功能</param>
    /// <param name="repository">仓储</param>
    /// <param name="modifyAsync">修改功能</param>
    /// <param name="createAsync">新增功能</param>
    /// <returns></returns>
    public static async Task ResetAsync<TEntity, TInput>(this IList<TEntity> entityList, IList<TInput> inputList, Func<TEntity, TInput, int, Task<TEntity>> createAsync, Func<TEntity, TInput, int, Task<TEntity>> modifyAsync = null, Func<List<Guid>, Task<bool>> deleteAsync = null, IRepository<TEntity, Guid> repository = null)
        where TEntity : class, IEntity<Guid>, new()
        where TInput : class, IHasId<Guid?>
    {
        //===============|| inputList.Count == 0
        if (inputList == null)
        {
            return;
            //throw new AbpException($"输入列表不能为Null");
        }
        var inputIdList = inputList.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();

        var allIdList = entityList.Select(x => x.Id).ToList();

        //删除的
        if (deleteAsync != null)
        {
            var deleteldIdList = allIdList.Except(inputIdList).ToList();

            if (deleteldIdList.Count > 0)
            {
                if (deleteAsync == null || await deleteAsync.Invoke(deleteldIdList))
                {
                    foreach (var deleteId in deleteldIdList)
                    {
                        var item = entityList.Single(x => x.Id == deleteId);

                        var index = entityList.IndexOf(item);

                        entityList.RemoveAt(index);
                    }
                    if (repository != null)
                    {
                        await repository.DeleteAsync(x => deleteldIdList.Contains(x.Id));
                    }
                }
            }
        }

        //修改的
        if (modifyAsync != null)
        {
            var modifyIdList = inputIdList.Intersect(allIdList).ToList();

            var modifyEntityList = entityList.Where(x => modifyIdList.Contains(x.Id)).ToList();

            foreach (var entity in modifyEntityList)
            {
                var inputField = inputList.FirstOrDefault(d => d.Id == entity.Id);

                if (inputField != null)
                {
                    var index = inputList.IndexOf(inputField);

                    await modifyAsync(entity, inputField, index);
                }
            }
        }
        //新增的
        if (createAsync != null)
        {
            var newFieldList = inputList.Where(x => !x.Id.HasValue).ToList();
            foreach (var inputField in newFieldList)
            {
                var index = inputList.IndexOf(inputField);

                var newEntity = await createAsync(new TEntity(), inputField, index);

                entityList.Add(newEntity);
            }
        }
    }
}
