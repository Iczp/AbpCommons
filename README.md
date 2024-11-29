# IczpNet.AbpCommons

An abp module that provides standard tree structure entity implement.

## Push to Nuget

1. 在项目根目录执行以下命令，将项目打包为 nuget 包

```bash
 scripts/pushToNuget.ps1
```

## Usage

Add `DependsOn(typeof(AbpCommonsXxxModule))` attribute to configure the module dependencies.

1. Application

```C#
[DependsOn(typeof(AbpCommonsApplicationModule))]
```

2. Application.Contracts

```C#
[DependsOn(typeof(AbpCommonsApplicationContractsModule))]
```

3. Domain.Shared

```C#
[DependsOn(typeof(AbpCommonsDomainSharedModule))]
```

4. EntityFrameworkCore

```C#
[DependsOn(typeof(AbpCommonsEntityFrameworkCoreModule))]
```

5. HttpApi

```C#
[DependsOn(typeof(AbpCommonsHttpApiModule))]
```

6. HttpApi.Client

```C#
[DependsOn(typeof(AbpCommonsHttpApiClientModule))]
```

7. Installer

```C#
[DependsOn(typeof(AbpCommonsInstallerModule))]
```

8. MongoDb

```C#
[DependsOn(typeof(AbpCommonsMongoDbModule))]
```

## Internal structure

### IczpNet.AbpCommons.Domain

#### `AbpCommonsDomainModule.cs`

```C#
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using IczpNet.AbpCommons.Extensions;

namespace IczpNet.AbpCommons;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCommonsDomainSharedModule)
)]
public class AbpCommonsDomainModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;

        app.UseStaticJsonSerializer();

        app.UseStaticAutoMapper();

        await base.OnApplicationInitializationAsync(context);
    }
}

```

### Attributes

`IczpNet.AbpCommons.Domain`

#### `HasKeysAttribute`

```C#
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp;

namespace IczpNet.AbpCommons.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HasKeysAttribute : Attribute

    {
        private bool? _isUnique;

        private string _name;

        //
        // 摘要:
        //     The properties which constitute the index, in order.
        public IReadOnlyList<string> PropertyNames { get; }

        //
        // 摘要:
        //     The name of the index.
        public string Name
        {
            get
            {
                return _name;
            }
            [param: DisallowNull]
            set
            {
                _name = Check.NotNull(value, "value");
            }
        }

        /// <summary>
        /// Whether the index is unique.
        /// </summary>
        public bool IsUnique
        {
            get
            {
                return _isUnique.GetValueOrDefault();
            }
            set
            {
                _isUnique = value;
            }
        }

        /// <summary>
        /// Checks whether IsUnique has been
        /// explicitly set to a value.
        /// </summary>
        public bool IsUniqueHasValue => _isUnique.HasValue;

        /// <summary>
        /// Initializes a new instance of the IndexAttribute
        /// </summary>
        /// <param name="propertyNames">The properties which constitute the index, in order(there must be at least one).</param>
        public HasKeysAttribute(params string[] propertyNames)
        {
            Assert.NotNull(propertyNames, null, "propertyNames");
            PropertyNames = propertyNames.ToList();
        }
    }
}

```

### DataFilters

#### ICode

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface ICode
    {
        string Code { get; }
    }
}

```

#### IHasId

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IHasId<TKey>
    {
        TKey Id { get; }
    }
}

```

#### IHasPinyin

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IHasPinyin
    {
        string Name_Py { get; }
        string Name_Pinyin { get; }
    }
}

```

#### IIsActive

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IIsActive
    {
        bool IsActive { get; }
    }
}

```

#### IIsPublic

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IIsPublic
    {
        bool IsPublic { get; }
    }
}

```

#### IIsStatic

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IIsStatic
    {
        bool IsStatic { get; }
    }
}

```

#### IKeyword

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IKeyword
    {
        string Keyword { get; }
    }
}

```

#### IName

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface IName
    {
        string Name { get; }
    }
}

```

#### ISorting

```C#
namespace IczpNet.AbpCommons.DataFilters
{
    public interface ISorting
    {
        double Sorting { get; }
    }
}

```

### Enums

#### ExpressTypeEnum

```C#
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

```

### Models

#### FilterInput

```C#
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IczpNet.AbpCommons.Models
{
    /// <summary>
    /// 字段过滤
    /// </summary>
    public class FilterInput
    {
        /// <summary>
        /// 字段Id
        /// </summary>
        public virtual Guid PropertyNameId { get; set; }
        /// <summary>
        /// 运算表达式{
        ///	0: "=",
        ///	1: ">",
        ///	2: ">=",
        ///	3: "&lt;",
        ///	4: "&lt;=",
        ///	5: "!=",
        ///	6: "like"
        ///}
        /// </summary>
        [Description("=,!=,>,>=,<,<=,%,!%")]
        public virtual string ExpressType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        [Required]
        public virtual string Value { get; set; }
        ///// <summary>
        ///// 字段过滤
        ///// </summary>
        //public virtual List<FilterInput> Filters { get; set; }
    }
}

```

#### ValueInput

```C#
using System.ComponentModel.DataAnnotations;

namespace IczpNet.AbpCommons.Models
{
    public class ValueInput
    {
        //[DefaultValue(null)]
        //public virtual Guid? Id { get; set; }

        [Required]
        public virtual string Value { get; set; }
    }
}

```

### Extensions

#### AutoMapperExtensions

```C#
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AutoMapper;

namespace IczpNet.AbpCommons.Extensions
{
    /// <summary>
    /// AutoMapperExtensions
    /// </summary>
    public static class AutoMapperExtensions
    {
        private static IServiceProvider ServiceProvider;
        private static IMapper _mapper;

        //private static IObjectMapper _objectMapper;
        //IMapperAccessor
        private static IMapper Mapper => _mapper ??= ServiceProvider.GetRequiredService<IMapperAccessor>().Mapper;
        //private static IMapper Mapper => _mapper ??= ServiceProvider.GetRequiredService<IMapper>();
        //private static IObjectMapper ObjectMapper => _objectMapper ??= ServiceProvider.GetRequiredService<IObjectMapper>();
        public static void UseStaticAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            ServiceProvider = applicationBuilder.ApplicationServices;
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) => Mapper.Map<TSource, TDestination>(source, destination);

        public static TDestination MapTo<TDestination>(this object source) => Mapper.Map<TDestination>(source);
    }
}

```

#### EnumerableExtensions

```C#
using System;
using System.Collections.Generic;
using System.Linq;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// EnumerableExtensions
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 从集合中的选出K个元素组合数
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> sequences, int k)
    {
        return k == 0 ? new[] { new T[0] } : sequences.SelectMany((e, i) => sequences.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }
    /// <summary>
    /// 求集合的笛卡尔积
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Cartesian<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> tempProduct = new[] { Enumerable.Empty<T>() };
        return sequences.Aggregate(tempProduct, (accumulator, sequence) =>
                                         from accseq in accumulator
                                         from item in sequence
                                         select accseq.Concat(new[] { item })
                                   );
    }

    private static void aa()
    {

        List<List<string>> items = new List<List<string>>()
        {
            new List<string>(){ "a1","a2","a3"},
            new List<string>(){ "b4","b5"},
            new List<string>(){ "c6" }
        };

        foreach (var item in items.Cartesian())
        {
            //console.WriteLine(string.Join(",", item));
        }
    }
    /// <summary>
    /// if null to empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }
    /// <summary>
    /// if null to ToList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<T> NullToList<T>(this IEnumerable<T> source)
    {
        return source.NullToEmpty().ToList();
    }
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> list)
    {
        return list.NullToEmpty().Any();
    }
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return list.NullToEmpty().Any(predicate);
    }
}

```

#### EnumExtension

```C#
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// 枚举工具类(Iczp.Net)
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 从枚举中获取Description
    /// 说明：
    /// 单元测试-->通过
    /// </summary>
    /// <param name="enumName">需要获取枚举描述的枚举</param>
    /// <returns>描述内容</returns>
    public static string GetDescription(this Enum enumName)
    {
        var field = enumName.GetType().GetField(enumName.ToString());
        var attributes = field.GetDescriptionAttributes();
        string description;
        if (attributes != null && attributes.Length > 0)
        {
            description = attributes[0].Description;
        }
        else
        {
            description = null;
        }
        return description;
    }

    ///// <summary>
    ///// get enum description by name
    ///// </summary>
    ///// <typeparam name="T">enum type</typeparam>
    ///// <param name="enumItemName">the enum name</param>
    ///// <returns></returns>
    //public static string GetDescription<T>(this T enumItemName)
    //{
    //    FieldInfo field = enumItemName.GetType().GetField(enumItemName.ToString());

    //    DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(
    //        typeof(DescriptionAttribute), false);

    //    if (attributes != null && attributes.Length > 0)
    //    {
    //        return attributes[0].Description;
    //    }
    //    else
    //    {
    //        return enumItemName.ToString();
    //    }
    //}
    /// <summary>
    /// 获取字段Description
    /// </summary>
    /// <param name="fieldInfo">FieldInfo</param>
    /// <returns>DescriptionAttribute[] </returns>
    public static DescriptionAttribute[] GetDescriptionAttributes(this FieldInfo fieldInfo)
    {
        if (fieldInfo != null)
        {
            return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        }
        return null;
    }
    /// <summary>
    /// 根据Description获取枚举
    /// 说明：
    /// 单元测试-->通过
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="description">枚举描述</param>
    /// <returns>枚举</returns>
    public static T AsEnum<T>(this string description)
    {
        var type = typeof(T);
        foreach (var field in type.GetFields())
        {
            var attributes = field.GetDescriptionAttributes();
            if (attributes != null && attributes.Length > 0)
            {
                if (attributes[0].Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            else
            {
                if (field.Name == description)
                {
                    return (T)field.GetValue(null);
                }
            }
        }
        throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
    }
    /// <summary>
    /// 将枚举转换为ArrayList
    /// 说明：
    /// 若不是枚举类型，则返回NULL
    /// 单元测试-->通过
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <returns>ArrayList</returns>
    public static ArrayList ToArrayList(this Type type)
    {
        if (type.IsEnum)
        {
            var _array = new ArrayList();
            var _enumValues = Enum.GetValues(type);
            foreach (Enum value in _enumValues)
            {
                _array.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
            }
            return _array;
        }
        return null;
    }
}

```

#### IsEmptyExtensions

```C#
using System;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// IsEmptyExtensions
/// </summary>
public static class IsEmptyExtensions
{
    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this Guid? value)
    {
        if (value == null)
            return true;
        return Guid.Empty.Equals(value);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this Guid value)
    {
        if (value == Guid.Empty)
            return true;
        return false;
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this object value)
    {
        if (value != null && !string.IsNullOrEmpty(value.ToString()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

```

#### JsonExtensions

```C#

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Json;

namespace IczpNet.AbpCommons.Extensions
{
    /// <summary>
    /// JsonExtensions
    /// </summary>
    public static class JsonExtensions
    {
        private static IServiceProvider ServiceProvider;
        private static IJsonSerializer _jsonSerializer;
        private static IJsonSerializer JsonSerializer => _jsonSerializer ??= ServiceProvider.GetRequiredService<IJsonSerializer>();
        public static void UseStaticJsonSerializer(this IApplicationBuilder applicationBuilder)
        {
            ServiceProvider = applicationBuilder.ApplicationServices;
        }
        public static T ToObject<T>(this string source) where T : class
        {
            return JsonSerializer.Deserialize<T>(source);
        }
        public static string ToJson(this object source, bool indented = false)
        {
            return JsonSerializer.Serialize(source, indented);
        }
    }
}

```

#### QueryExtensions

```C#
using System;
using System.Linq;
using System.Linq.Expressions;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// QueryExtensions
/// </summary>
public static class QueryExtensions
{
    private const string SORT_DIRECTION_DESC = " DESC";
    /// <summary>
    /// SortBy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sortExpression"></param>
    /// <returns></returns>
    public static IQueryable<T> SortingBy<T>(this IQueryable<T> source, string sortExpression) where T : class
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (string.IsNullOrWhiteSpace(sortExpression))
        {
            return source;
        }

        sortExpression = sortExpression.Trim();
        bool flag = false;
        if (sortExpression.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            flag = true;
            int length = sortExpression.Length - " DESC".Length;
            sortExpression = sortExpression.Substring(0, length).Trim();
        }

        if (string.IsNullOrEmpty(sortExpression))
        {
            return source;
        }

        ParameterExpression parameterExpression = Expression.Parameter(source.ElementType, string.Empty);
        MemberExpression memberExpression = null;
        string[] array = sortExpression.Split('.');
        string[] array2 = array;
        foreach (string propertyName in array2)
        {
            memberExpression = ((memberExpression != null) ? Expression.Property(memberExpression, propertyName) : Expression.Property(parameterExpression, propertyName));
        }

        LambdaExpression expression = Expression.Lambda(memberExpression, parameterExpression);
        string methodName = flag ? "OrderByDescending" : "OrderBy";
        Expression expression2 = Expression.Call(typeof(Queryable), methodName, new Type[2]
        {
            source.ElementType,
            memberExpression.Type
        }, source.Expression, Expression.Quote(expression));
        return (IQueryable<T>)source.Provider.CreateQuery(expression2);
    }

}

```

#### ResetListExtension

```C#
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

```

#### StringExtension

```C#
namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// 枚举工具类(Iczp.Net)
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 字符限制最大长度
    /// </summary>
    /// <param name="input"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string MaxLength(this string input, int maxLength)
    {
        return input.Length > maxLength ? input.Substring(0, maxLength) : input;
    }
}

```

### EntityFrameworkCore `ConfigEntitysExtensions`

IczpNet.AbpCommons.EntityFrameworkCore

```C#
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons.EntityFrameworkCore
{
    public static class ConfigEntitysExtensions
    {
        public static void ConfigEntitys<T>(this ModelBuilder builder, string dbTablePrefix, string dbSchema) where T : AbpModule
        {
            builder.ConfigEntitys(typeof(T), dbTablePrefix, dbSchema);
        }
        public static void ConfigEntitys<T>(this ModelBuilder builder, Func<Type, string> getTableName, string dbSchema) where T : AbpModule
        {
            builder.ConfigEntitys(typeof(T), getTableName, dbSchema);
        }

        public static void ConfigEntitys(this ModelBuilder builder, Type moduleType, string dbTablePrefix, string dbSchema)
        {
            builder.ConfigEntitys(moduleType, entityType => dbTablePrefix + "_" + entityType.Name, dbSchema);
        }

        public static void ConfigEntitys(this ModelBuilder builder, Type moduleType, Func<Type, string> getTableName, string dbSchema)
        {
            var entityNamespace = moduleType.Namespace;

            var entityTypes = moduleType.Assembly.GetExportedTypes()
                .Where(t => t.Namespace.StartsWith(entityNamespace) && !t.IsAbstract
                    && t.GetInterfaces().Any(x => typeof(IEntity).IsAssignableFrom(x) || x.IsGenericType && typeof(IEntity<>).IsAssignableFrom(x.GetGenericTypeDefinition())));

            foreach (var t in entityTypes)
            {
                builder.Entity(t, b =>
                {
                    var tableAttribute = t.GetCustomAttribute<TableAttribute>();

                    if (tableAttribute != null)
                    {
                        b.ToTable(tableAttribute.Name, tableAttribute.Schema);
                    }
                    else
                    {
                        b.ToTable(getTableName(t), dbSchema);
                    }

                    AbpEntityTypeBuilderExtensions.ConfigureByConvention(b); //auto configure for the base class props

                    b.ConfigureByConvention();
                });
            }
        }
    }
}

```

### IczpNet.AbpCommons.Application

#### CrudCommonAppService

`CrudCommonAppService.cs`

```C#
using IczpNet.AbpCommons.DataFilters;
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
        public override async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            await CheckCreatePolicyAsync();

            var entity = await MapToEntityAsync(input);

            await SetCreateEntityAsync(entity, input);

            TryToSetTenantId(entity);

            await Repository.InsertAsync(entity, autoSave: true);

            return await MapToGetOutputDtoAsync(entity);
        }

        protected virtual Task SetCreateEntityAsync(TEntity entity, TCreateInput input)
        {
            return Task.CompletedTask;
        }

        [HttpPost]
        public override async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            await CheckUpdatePolicyAsync();

            var entity = await GetEntityByIdAsync(id);
            //TODO: Check if input has id different than given id and normalize if it's default value, throw ex otherwise
            await MapToEntityAsync(input, entity);

            await SetUpdateEntityAsync(entity, input);

            await Repository.UpdateAsync(entity, autoSave: true);

            return await MapToGetOutputDtoAsync(entity);
        }

        protected virtual Task SetUpdateEntityAsync(TEntity entity, TUpdateInput input)
        {
            return Task.CompletedTask;
        }

        [HttpPost]
        public override async Task DeleteAsync(TKey id)
        {
            await CheckDeleteIsStaticAsync(id);

            await base.DeleteAsync(id);
        }

        protected virtual async Task CheckDeleteIsStaticAsync(TKey id)
        {
            var entity = await GetEntityByIdAsync(id);

            var propInfo = entity.GetType().GetProperty(nameof(IIsStatic.IsStatic));

            Assert.If(entity is IIsStatic && propInfo != null && (bool)propInfo.GetValue(entity), "IsStatic=True,cannot delete.");
        }

        [HttpPost]
        public virtual async Task DeleteManyAsync(List<TKey> idList)
        {
            foreach (var id in idList)
            {
                await DeleteAsync(id);
            }
        }
    }
}

```

## RepositoryUrl

https://github.com/Iczp/AbpCommons.git

## PackageProjectUrl

https://github.com/Iczp/AbpCommons.git

## Utils

### ImageHelper

```c#
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace IczpNet.AbpCommons.Utils;

/// <summary>
/// 图片处理类- by Iczp.net
/// </summary>
public static class ImageHelper
{
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="path">虚拟路径，如：C:\\uploads</param>
    /// <returns></returns>
    public static string CreateDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        //var dir = HostingEnvironment.MapPath(path);
        var paths = dir.Split('\\');
        var pathList = new List<string>()
        {
            paths[0]
        };
        for (var i = 1; i < paths.Length; i++)
        {
            pathList.Add(paths[i]);
            var dirPath = string.Join("\\", pathList);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        return dir;
    }

    public static byte[] GetEmptyPngBytes(int width, int height)
    {
        using var bitmap = new Bitmap(width, height);
        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    /// <summary>
    /// 设置圆角
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <param name="cornerRadius">圆角的半径</param>
    /// <returns></returns>
    public static byte[] SetImageRoundCorner(byte[] imageBytes, int cornerRadius)
    {
        using var ms = new MemoryStream(imageBytes);
        using var originalImage = Image.FromStream(ms);
        using var roundedImage = new Bitmap(originalImage.Width, originalImage.Height);
        using (var path = new GraphicsPath())
        {
            path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();

            using var graphics = Graphics.FromImage(roundedImage);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawImage(originalImage, new Rectangle(0, 0, roundedImage.Width, roundedImage.Height));
            graphics.DrawPath(new Pen(Color.Transparent), path);
            graphics.FillPath(new SolidBrush(Color.Transparent), path);
        }

        using var ms2 = new MemoryStream();
        roundedImage.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
        return ms2.ToArray();
    }

    /// <summary>
    /// 九宫格图片 by Iczp.net
    /// </summary>
    /// <param name="imageBytes">图片</param>
    /// <param name="size">大小（宽高）</param>
    /// <param name="cornerRadius">圆角的半径</param>
    /// <param name="gap">间距</param>
    /// <returns></returns>
    public static byte[] MakeMergeThumbnails(List<byte[]> imageBytes, int size, int gap = 10, int cornerRadius = 10)
    {
        // 新建一个bmp图片
        var bitmap = new Bitmap(size, size);
        // 新建一个画板
        var graphics = Graphics.FromImage(bitmap);
        // 设置高质量插值法
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        // 设置高质量,低速度呈现平滑程度
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        // 清空一下画布
        graphics.Clear(Color.Transparent);

        // 在指定位置画图
        var stup = (bitmap.Width - gap) / 3;
        var index = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (index >= imageBytes.Count)
                {
                    break;
                }

                var imageData = imageBytes[index];

                //if (cornerRadius > 0)
                //{
                //    imageData = SetImageRoundCorner(imageData, cornerRadius);
                //}

                index++;

                // 画布区域
                var destRect = new Rectangle(stup * j + gap, stup * i + gap, stup - gap, stup - gap);
                // 图片区域
                using var stream = new MemoryStream(imageData);
                using var myImage = Image.FromStream(stream);
                int x = 0, y = 0;
                int minValue = (myImage.Width >= myImage.Height) ? myImage.Height : myImage.Width;
                if (myImage.Width >= myImage.Height)
                {
                    x = (myImage.Width - myImage.Height) / 2;
                }
                else
                {
                    y = (myImage.Height - myImage.Width) / 2;
                }
                var srcRect = new Rectangle(x, y, minValue, minValue);
                graphics.DrawImage(myImage, destRect, srcRect, GraphicsUnit.Pixel);
                // 设置圆角
                if (cornerRadius > 0)
                {
                    using var path = new GraphicsPath();
                    path.AddArc(destRect.X, destRect.Y, cornerRadius, cornerRadius, 180, 90);
                    path.AddArc(destRect.X + destRect.Width - cornerRadius, destRect.Y, cornerRadius, cornerRadius, 270, 90);
                    path.AddArc(destRect.X + destRect.Width - cornerRadius, destRect.Y + destRect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    path.AddArc(destRect.X, destRect.Y + destRect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    path.CloseAllFigures();

                    using var region = new Region(path);
                    graphics.SetClip(region, CombineMode.Exclude);
                    graphics.Clear(Color.Transparent);
                }
            }
        }

        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// 九宫格图片 by Iczp.net
    /// </summary>
    /// <param name="filePaths">图片物理路径</param>
    /// <param name="savePath">保存路径</param>
    /// <param name="widthAndHeight">宽高（正方形图片）</param>
    /// <param name="margin">间距</param>
    public static void MakeMergeThumbnails(List<string> filePaths, string savePath, int widthAndHeight, int margin = 10)
    {
        //新建一个bmp图片
        Image bitmap = new Bitmap(widthAndHeight, widthAndHeight);
        //新建一个画板
        Graphics graphics = Graphics.FromImage(bitmap);
        //设置高质量插值法
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //设置高质量,低速度呈现平滑程度
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //清空一下画布
        graphics.Clear(Color.Transparent);
        //在指定位置画图
        //var margin = 10;
        var stup = (bitmap.Width - margin) / 3;
        var index = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (index >= filePaths.Count())
                {
                    break;
                }
                var imagePath = filePaths[index];
                index++;
                if (!File.Exists(imagePath))
                {
                    continue;
                }
                var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //var data = fileStream.GetAllBytesAsync();
                //画布区域
                var destRect = new Rectangle(stup * j + margin, stup * i + margin, stup - margin, stup - margin);
                //图片区域
                Image myImage = Image.FromStream(fileStream);
                int x = 0, y = 0;
                int minValue = (myImage.Width >= myImage.Height) ? myImage.Height : myImage.Width;
                if (myImage.Width >= myImage.Height)
                {
                    x = (myImage.Width - myImage.Height) / 2;
                }
                else
                {
                    y = (myImage.Height - myImage.Width) / 2;
                }
                var srcRect = new Rectangle(x, y, minValue, minValue);
                graphics.DrawImage(myImage, destRect, srcRect, GraphicsUnit.Pixel);
                myImage.Clone();
                myImage.Dispose();
                fileStream.Dispose();
            }
        }
        bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
        //using (MemoryStream memoryStream = new MemoryStream())
        //{
        //    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        //    memoryStream.Seek(0, SeekOrigin.Begin); // otherwise you'll get zero byte files
        //}
        graphics.Dispose();
        bitmap.Dispose();
    }
    /// <summary>
    /// 设置圆角
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="filePath"></param>
    /// <param name="newFileName"></param>
    /// <param name="newWidth"></param>
    /// <param name="newHeight"></param>
    public static void MakeRoundedThumbnails(Stream fileStream, string filePath, string newFileName, int newWidth, int newHeight)
    {
        Image myImage = Image.FromStream(fileStream);
        //先将图片处理为圆角
        myImage = DrawTransparentRoundCornerImage(myImage);
        //取得图片大小
        Size mySize = new Size(newWidth, newHeight);
        //新建一个bmp图片
        Image bitmap = new Bitmap(mySize.Width, mySize.Height);
        //新建一个画板
        Graphics graphics = Graphics.FromImage(bitmap);
        //设置高质量插值法
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //设置高质量,低速度呈现平滑程度
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //清空一下画布
        graphics.Clear(Color.Transparent);
        //在指定位置画图
        var margin = 10;
        var stup = (bitmap.Width - margin) / 3;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                graphics.DrawImage(myImage, new Rectangle(stup * i + margin, stup * j + margin, stup - margin, stup - margin), new Rectangle(myImage.Width / 3 * i, myImage.Height / 3 * j, myImage.Width / 3, myImage.Height / 3), GraphicsUnit.Pixel);
            }
        }
        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        graphics.Dispose();
        myImage.Clone();
        myImage.Dispose();
        bitmap.Dispose();
    }

    /// <summary>
    /// 图片处理为圆角
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Image DrawTransparentRoundCornerImage(System.Drawing.Image image)
    {
        Bitmap bm = new Bitmap(image.Width, image.Height);
        Graphics g = Graphics.FromImage(bm);
        g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, image.Width, image.Height));

        using (System.Drawing.Drawing2D.GraphicsPath path = CreateRoundedRectanglePath(new Rectangle(0, 0, image.Width, image.Height), image.Width / 10))
        {
            g.SetClip(path);
        }

        g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        g.Dispose();

        return bm;
    }
    //设置图片四个边角弧度
    private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
    {
        System.Drawing.Drawing2D.GraphicsPath roundedRect = new System.Drawing.Drawing2D.GraphicsPath();
        roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
        roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
        roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
        roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
        roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
        roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
        roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
        roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
        roundedRect.CloseFigure();
        return roundedRect;
    }
    /// <summary>
    /// 无损压缩图片
    /// </summary>
    /// <param name="source">原图片地址</param>
    /// <param name="target">压缩后保存图片地址</param>
    /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
    /// <param name="maxLength">压缩后图片的最大大小</param>
    /// <param name="sfsc">是否是第一次调用</param>
    /// <returns></returns>
    public static bool CompressImage(string source, string target, int flag = 90, int maxLength = 300, bool sfsc = true)
    {
        //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
        FileInfo firstFileInfo = new FileInfo(source);
        if (sfsc == true && firstFileInfo.Length < maxLength * 1024)
        {
            firstFileInfo.CopyTo(target);
            return true;
        }
        Image iSource = Image.FromFile(source);
        ImageFormat tFormat = iSource.RawFormat;
        int dHeight = iSource.Height / 2;
        int dWidth = iSource.Width / 2;
        int sW = 0, sH = 0;
        //按比例缩放
        Size tem_size = new Size(iSource.Width, iSource.Height);
        if (tem_size.Width > dHeight || tem_size.Width > dWidth)
        {
            if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
            {
                sW = dWidth;
                sH = (dWidth * tem_size.Height) / tem_size.Width;
            }
            else
            {
                sH = dHeight;
                sW = (tem_size.Width * dHeight) / tem_size.Height;
            }
        }
        else
        {
            sW = tem_size.Width;
            sH = tem_size.Height;
        }

        Bitmap bitmap = new Bitmap(dWidth, dHeight);
        Graphics g = Graphics.FromImage(bitmap);

        g.Clear(Color.WhiteSmoke);
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

        g.Dispose();

        //以下代码为保存图片时，设置压缩质量
        EncoderParameters ep = new EncoderParameters();
        long[] qy = new long[1];
        qy[0] = flag;//设置压缩的比例1-100
        EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
        ep.Param[0] = eParam;

        try
        {
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICIinfo = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICIinfo = arrayICI[x];
                    break;
                }
            }
            if (jpegICIinfo != null)
            {
                bitmap.Save(target, jpegICIinfo, ep);//target是压缩后的新路径
                FileInfo fi = new FileInfo(target);
                if (fi.Length > 1024 * maxLength)
                {
                    flag = flag - 10;
                    CompressImage(source, target, flag, maxLength, false);
                }
            }
            else
            {
                bitmap.Save(target, tFormat);
            }
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            iSource.Dispose();
            bitmap.Dispose();
        }
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="source"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static Stream CompressImageByMaxLength(Stream source, int maxLength = 3001024)
    {
        int flag = 90;
        var result = source;
        while (result.Length > maxLength && flag > 10)
        {
            flag -= 10;
            result = CompressImage(source, flag);
        }
        return result;
    }

    /// <summary>
    /// 无损压缩图片
    /// </summary>
    /// <param name="source">原图片地址</param>
    /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
    /// <returns></returns>
    public static Stream CompressImage(Stream source, int flag = 90)
    {
        Image iSource = Image.FromStream(source);
        ImageFormat tFormat = iSource.RawFormat;
        int dHeight = iSource.Height / 2;
        int dWidth = iSource.Width / 2;
        int sW = 0, sH = 0;
        //按比例缩放
        Size tem_size = new Size(iSource.Width, iSource.Height);
        if (tem_size.Width > dHeight || tem_size.Width > dWidth)
        {
            if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
            {
                sW = dWidth;
                sH = (dWidth * tem_size.Height) / tem_size.Width;
            }
            else
            {
                sH = dHeight;
                sW = (tem_size.Width * dHeight) / tem_size.Height;
            }
        }
        else
        {
            sW = tem_size.Width;
            sH = tem_size.Height;
        }

        Bitmap bitmap = new Bitmap(dWidth, dHeight);
        Graphics g = Graphics.FromImage(bitmap);

        g.Clear(Color.WhiteSmoke);
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

        g.Dispose();

        //以下代码为保存图片时，设置压缩质量
        EncoderParameters ep = new EncoderParameters();
        long[] qy = new long[1];
        qy[0] = flag;//设置压缩的比例1-100
        EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
        ep.Param[0] = eParam;

        Stream stream = new MemoryStream();
        try
        {
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICIinfo = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICIinfo = arrayICI[x];
                    break;
                }
            }
            if (jpegICIinfo != null)
            {
                bitmap.Save(stream, jpegICIinfo, ep);
                //bitmap.Save(@"F:\uploads\5555.jpg", jpegICIinfo, ep);
            }
            else
            {
                bitmap.Save(stream, tFormat);
                //bitmap.Save(@"F:\uploads\4444.jpg", tFormat);
            }

            return stream;
            //return await stream.GetAllBytesAsync();
        }
        catch
        {
            //bitmap.Save(@"F:\uploads\3333.jpg", tFormat);
            return source;
        }
        finally
        {
            iSource.Dispose();
            bitmap.Dispose();
        }
    }



    #region 正方型裁剪并缩放

    /// <summary>
    /// 正方型裁剪
    /// 以图片中心为轴心，截取正方型，然后等比缩放
    /// 用于头像处理
    /// </summary>
    /// <remarks>吴剑 2012-08-08</remarks>
    /// <param name="fromFile">原图Stream对象</param>
    /// <param name="fileSaveUrl">缩略图存放地址</param>
    /// <param name="side">指定的边长（正方型）</param>
    /// <param name="quality">质量（范围0-100）</param>
    public static void CutForSquare(System.IO.Stream fromFile, string fileSaveUrl, int side, int quality)
    {
        //创建目录
        string dir = Path.GetDirectoryName(fileSaveUrl);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
        System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

        //原图宽高均小于模版，不作处理，直接保存
        if (initImage.Width <= side && initImage.Height <= side)
        {
            initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        else
        {
            //原始图片的宽、高
            int initWidth = initImage.Width;
            int initHeight = initImage.Height;

            //非正方型先裁剪为正方型
            if (initWidth != initHeight)
            {
                //截图对象
                System.Drawing.Image pickedImage = null;
                System.Drawing.Graphics pickedG = null;

                //宽大于高的横图
                if (initWidth > initHeight)
                {
                    //对象实例化
                    pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //定位
                    Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                    Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                    //画图
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                    //重置宽
                    initWidth = initHeight;
                }
                //高大于宽的竖图
                else
                {
                    //对象实例化
                    pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //定位
                    Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                    Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                    //画图
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                    //重置高
                    initHeight = initWidth;
                }

                //将截图对象赋给原图
                initImage = (System.Drawing.Image)pickedImage.Clone();
                //释放截图资源
                pickedG.Dispose();
                pickedImage.Dispose();
            }

            //缩略图对象
            System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
            System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
            //设置质量
            resultG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resultG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //用指定背景色清空画布
            resultG.Clear(Color.White);
            //绘制缩略图
            resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

            //关键质量控制
            //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
            ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo i in icis)
            {
                if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                {
                    ici = i;
                }
            }
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

            //保存缩略图
            resultImage.Save(fileSaveUrl, ici, ep);

            //释放关键质量控制所用资源
            ep.Dispose();

            //释放缩略图资源
            resultG.Dispose();
            resultImage.Dispose();

            //释放原始图片资源
            initImage.Dispose();
        }
    }

    #endregion

    #region 自定义裁剪并缩放

    /// <summary>
    /// 指定长宽裁剪
    /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
    /// </summary>
    /// <remarks>吴剑 2012-08-08</remarks>
    /// <param name="fromFile">原图Stream对象</param>
    /// <param name="fileSaveUrl">保存路径</param>
    /// <param name="maxWidth">最大宽(单位:px)</param>
    /// <param name="maxHeight">最大高(单位:px)</param>
    /// <param name="quality">质量（范围0-100）</param>
    public static void CutForCustom(System.IO.Stream fromFile, string fileSaveUrl, int maxWidth, int maxHeight, int quality)
    {
        //从文件获取原始图片，并使用流中嵌入的颜色管理信息
        System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

        //原图宽高均小于模版，不作处理，直接保存
        if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
        {
            initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        else
        {
            //模版的宽高比例
            double templateRate = (double)maxWidth / maxHeight;
            //原图片的宽高比例
            double initRate = (double)initImage.Width / initImage.Height;

            //原图与模版比例相等，直接缩放
            if (templateRate == initRate)
            {
                //按模版大小生成最终图片
                System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                templateG.Clear(Color.White);
                templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            //原图与模版比例不等，裁剪后缩放
            else
            {
                //裁剪对象
                System.Drawing.Image pickedImage = null;
                System.Drawing.Graphics pickedG = null;

                //定位
                Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                //宽为标准进行裁剪
                if (templateRate > initRate)
                {
                    //裁剪对象实例化
                    pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                    //裁剪源定位
                    fromR.X = 0;
                    fromR.Y = (int)System.Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                    fromR.Width = initImage.Width;
                    fromR.Height = (int)System.Math.Floor(initImage.Width / templateRate);

                    //裁剪目标定位
                    toR.X = 0;
                    toR.Y = 0;
                    toR.Width = initImage.Width;
                    toR.Height = (int)System.Math.Floor(initImage.Width / templateRate);
                }
                //高为标准进行裁剪
                else
                {
                    pickedImage = new System.Drawing.Bitmap((int)System.Math.Floor(initImage.Height * templateRate), initImage.Height);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                    fromR.X = (int)System.Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                    fromR.Y = 0;
                    fromR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                    fromR.Height = initImage.Height;

                    toR.X = 0;
                    toR.Y = 0;
                    toR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                    toR.Height = initImage.Height;
                }

                //设置质量
                pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //裁剪
                pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                //按模版大小生成最终图片
                System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                templateG.Clear(Color.White);
                templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                //关键质量控制
                //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
                ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo i in icis)
                {
                    if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                    {
                        ici = i;
                    }
                }
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

                //保存缩略图
                templateImage.Save(fileSaveUrl, ici, ep);
                //templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

                //释放资源
                templateG.Dispose();
                templateImage.Dispose();

                pickedG.Dispose();
                pickedImage.Dispose();
            }
        }

        //释放资源
        initImage.Dispose();
    }
    #endregion

    #region 等比缩放

    /// <summary>
    /// 图片等比缩放
    /// </summary>
    /// <remarks>吴剑 2012-08-08</remarks>
    /// <param name="fromFile">原图Stream对象</param>
    /// <param name="savePath">缩略图存放地址</param>
    /// <param name="targetWidth">指定的最大宽度</param>
    /// <param name="targetHeight">指定的最大高度</param>
    /// <param name="watermarkText">水印文字(为""表示不使用水印)</param>
    /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param>
    public static void ZoomAuto(System.IO.Stream fromFile, string savePath, System.Double targetWidth, System.Double targetHeight, string watermarkText, string watermarkImage)
    {
        //创建目录
        string dir = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
        System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);

        //原图宽高均小于模版，不作处理，直接保存
        if (initImage.Width <= targetWidth && initImage.Height <= targetHeight)
        {
            //文字水印
            if (watermarkText != "")
            {
                using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
                {
                    System.Drawing.Font fontWater = new Font("黑体", 10);
                    System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                    gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                    gWater.Dispose();
                }
            }

            //透明图片水印
            if (watermarkImage != "")
            {
                if (File.Exists(watermarkImage))
                {
                    //获取水印图片
                    using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                    {
                        //水印绘制条件：原始图片宽高均大于或等于水印图片
                        if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                        {
                            Graphics gWater = Graphics.FromImage(initImage);

                            //透明属性
                            ImageAttributes imgAttributes = new ImageAttributes();
                            ColorMap colorMap = new ColorMap();
                            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                            ColorMap[] remapTable = { colorMap };
                            imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                            float[][] colorMatrixElements = {
                               new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                               new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                            };

                            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                            imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                            gWater.Dispose();
                        }
                        wrImage.Dispose();
                    }
                }
            }

            //保存
            initImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        else
        {
            //缩略图宽、高计算
            double newWidth = initImage.Width;
            double newHeight = initImage.Height;

            //宽大于高或宽等于高（横图或正方）
            if (initImage.Width > initImage.Height || initImage.Width == initImage.Height)
            {
                //如果宽大于模版
                if (initImage.Width > targetWidth)
                {
                    //宽按模版，高按比例缩放
                    newWidth = targetWidth;
                    newHeight = initImage.Height * (targetWidth / initImage.Width);
                }
            }
            //高大于宽（竖图）
            else
            {
                //如果高大于模版
                if (initImage.Height > targetHeight)
                {
                    //高按模版，宽按比例缩放
                    newHeight = targetHeight;
                    newWidth = initImage.Width * (targetHeight / initImage.Height);
                }
            }

            //生成新图
            //新建一个bmp图片
            System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
            //新建一个画板
            System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);

            //设置质量
            newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //置背景色
            newG.Clear(Color.White);
            //画图
            newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

            //文字水印
            if (watermarkText != "")
            {
                using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(newImage))
                {
                    System.Drawing.Font fontWater = new Font("宋体", 10);
                    System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                    gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                    gWater.Dispose();
                }
            }

            //透明图片水印
            if (watermarkImage != "")
            {
                if (File.Exists(watermarkImage))
                {
                    //获取水印图片
                    using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                    {
                        //水印绘制条件：原始图片宽高均大于或等于水印图片
                        if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                        {
                            Graphics gWater = Graphics.FromImage(newImage);

                            //透明属性
                            ImageAttributes imgAttributes = new ImageAttributes();
                            ColorMap colorMap = new ColorMap();
                            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                            ColorMap[] remapTable = { colorMap };
                            imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                            float[][] colorMatrixElements = {
                               new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                               new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5
                               new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                            };

                            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                            imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                            gWater.Dispose();
                        }
                        wrImage.Dispose();
                    }
                }
            }

            //保存缩略图
            newImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            //释放资源
            newG.Dispose();
            newImage.Dispose();
            initImage.Dispose();
        }
    }

    #endregion

    #region 其它

    /// <summary>
    /// 判断文件类型是否为WEB格式图片
    /// (注：JPG,GIF,BMP,PNG)
    /// </summary>
    /// <param name="contentType">HttpPostedFile.ContentType</param>
    /// <returns></returns>
    public static bool IsWebImage(string contentType)
    {
        if (contentType == "image/pjpeg" || contentType == "image/jpeg" || contentType == "image/gif" || contentType == "image/bmp" || contentType == "image/png")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

}//end class


```

### JsonHelper

```c#
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;

namespace IczpNet.AbpCommons.Utils;

public static class JsonHelper
{
    public static List<string> ParseToList(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }
        using var document = JsonDocument.Parse(json);

        var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());

        foreach (var element in document.RootElement.EnumerateArray())
        {
            var value = element.GetString();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            builder.Add(value);
        }

        return [.. builder];
    }
}

```

### ReflectHelper

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IczpNet.AbpCommons.Utils;

public static class ReflectHelper
{
    public static Dictionary<string, object> GetConstantsTreeDictionary(Type constantsType)
    {
        var result = new Dictionary<string, object>();
        PopulateTreeDictionary(constantsType, result);
        return result;
    }

    public static Dictionary<string, object> GetConstantsTreeDictionary<T>()
    {
        return GetConstantsTreeDictionary(typeof(T));
    }

    private static void PopulateTreeDictionary(Type type, Dictionary<string, object> result)
    {
        foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            var nestedDictionary = new Dictionary<string, object>();

            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    nestedDictionary.Add(field.Name, (string)field.GetValue(null));
                }
            }

            // 递归调用以处理嵌套类型
            PopulateTreeDictionary(nestedType, nestedDictionary);

            result.Add(nestedType.Name, nestedDictionary);
        }
    }

    public static Dictionary<string, string> GetConstantsFlatDictionary(Type constantsType)
    {
        var result = new Dictionary<string, string>();
        PopulateFlatDictionary(constantsType, result, [constantsType.Name]);
        return result;
    }

    public static Dictionary<string, string> GetConstantsFlatDictionary<T>()
    {
        return GetConstantsFlatDictionary(typeof(T));
    }

    private static void PopulateFlatDictionary(Type type, Dictionary<string, string> result, List<string> parentPath)
    {
        foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            var currentPaths = new List<string>(parentPath) { nestedType.Name };

            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var fullPath = currentPaths.Concat([field.Name]).JoinAsString(".");
                    result[fullPath] = (string)field.GetValue(null);
                }
            }

            // 递归调用以处理嵌套类型
            PopulateFlatDictionary(nestedType, result, currentPaths);
        }
    }
}

```
