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
