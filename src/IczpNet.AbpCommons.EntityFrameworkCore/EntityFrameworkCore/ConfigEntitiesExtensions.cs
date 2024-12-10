using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons.EntityFrameworkCore;

public static class ConfigEntitiesExtensions
{
    public static void ConfigEntities<T>(this ModelBuilder builder, string dbTablePrefix, string dbSchema, Action<EntityTypeBuilder> action = null) where T : AbpModule
    {
        builder.ConfigEntities(typeof(T), dbTablePrefix, dbSchema, action);
    }
    public static void ConfigEntities<T>(this ModelBuilder builder, Func<Type, string> getTableName, string dbSchema, Action<EntityTypeBuilder> action = null) where T : AbpModule
    {
        builder.ConfigEntities(typeof(T), getTableName, dbSchema, action);
    }

    public static void ConfigEntities(this ModelBuilder builder, Type moduleType, string dbTablePrefix, string dbSchema, Action<EntityTypeBuilder> action = null)
    {
        builder.ConfigEntities(moduleType, entityType => dbTablePrefix + "_" + entityType.Name, dbSchema, action);
    }

    public static void ConfigEntities(this ModelBuilder builder, Type moduleType, Func<Type, string> getTableName, string dbSchema, Action<EntityTypeBuilder> action = null)
    {
        var entityNamespace = moduleType.Namespace;

        var entityTypes = moduleType.Assembly.GetExportedTypes()
            .Where(t => t.Namespace.StartsWith(entityNamespace) && !t.IsAbstract
                && t.GetInterfaces().Any(x => typeof(IEntity).IsAssignableFrom(x) || x.IsGenericType && typeof(IEntity<>).IsAssignableFrom(x.GetGenericTypeDefinition())))
            .Where(t => t.GetCustomAttribute<NotMappedAttribute>() == null);

        foreach (var t in entityTypes)
        {
            builder.Entity(t, b =>
            {
                var tableAttribute = t.GetCustomAttribute<TableAttribute>();

                var commentAttribute = t.GetCustomAttribute<CommentAttribute>();

                var tabName = tableAttribute != null ? tableAttribute.Name : getTableName(t);

                var tabSchema = tableAttribute != null ? tableAttribute.Schema : dbSchema;

                b.ToTable(tabName, tabSchema, t =>
                {
                    if (commentAttribute != null)
                    {
                        t.HasComment(commentAttribute.Comment);
                    }
                });

                //注释
                b.ConfigureComments();

                //设置默认值
                b.ConfigureDefaultValues();

                b.ConfigureByConvention();

                action?.Invoke(b);
            });
        }
    }

    /// <summary>
    /// 注释
    /// </summary>
    /// <param name="b"></param>
    public static void ConfigureComments(this EntityTypeBuilder b)
    {
        //字段注释
        foreach (var p in b.Metadata.GetDeclaredProperties())
        {
            var propDesc = p.PropertyInfo?.GetSingleAttributeOrNull<CommentAttribute>();

            if (propDesc == null)
            {
                continue;
            }

            b.Property(p.Name).HasComment(propDesc.Comment);
        }

        //导航属性注释
        foreach (var fk in b.Metadata.GetDeclaredForeignKeys())
        {
            foreach (var fkProp in fk.Properties)
            {
                var fkDesc = fk.GetNavigation(true)?.PropertyInfo?.GetSingleAttributeOrNull<CommentAttribute>();
                if (fkDesc == null)
                {
                    continue;
                }
                fkProp.SetComment(fkDesc.Comment);
            }
        }
    }

    /// <summary>
    /// 设置默认值
    /// </summary>
    /// <param name="b"></param>
    public static void ConfigureDefaultValues(this EntityTypeBuilder b)
    {
        //字段默认值
        foreach (var p in b.Metadata.GetDeclaredProperties())
        {
            var propDefaultValue = p.PropertyInfo?.GetSingleAttributeOrNull<DefaultValueAttribute>();

            if (propDefaultValue != null && propDefaultValue.Value != null)
            {
                b.Property(p.Name).HasDefaultValue(propDefaultValue.Value);
                continue;
            }
        }
    }
}
