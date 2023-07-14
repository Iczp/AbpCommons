# IczpNet.AbpCommons

An abp module that provides standard tree structure entity implement.

#### Public to Nuget.org

```bash
dotnet nuget push "src/*/bin/Release/*0.1.21.nupkg" --skip-duplicate -k xxxxxxxxxxxxxxx --source https://api.nuget.org/v3/index.json
```

### 

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

####  ICode

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
