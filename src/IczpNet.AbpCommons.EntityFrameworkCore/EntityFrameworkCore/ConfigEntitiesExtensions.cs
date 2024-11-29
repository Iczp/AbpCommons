using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Modularity;

namespace IczpNet.AbpCommons.EntityFrameworkCore;

public static class ConfigEntitiesExtensions
{
    public static void ConfigEntitys<T>(this ModelBuilder builder, string dbTablePrefix, string dbSchema) where T : AbpModule
    {
        builder.ConfigEntities(typeof(T), dbTablePrefix, dbSchema);
    }
    public static void ConfigEntities<T>(this ModelBuilder builder, Func<Type, string> getTableName, string dbSchema) where T : AbpModule
    {
        builder.ConfigEntities(typeof(T), getTableName, dbSchema);
    }

    public static void ConfigEntities(this ModelBuilder builder, Type moduleType, string dbTablePrefix, string dbSchema)
    {
        builder.ConfigEntities(moduleType, entityType => dbTablePrefix + "_" + entityType.Name, dbSchema);
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

                b.ConfigureComments();

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
}
