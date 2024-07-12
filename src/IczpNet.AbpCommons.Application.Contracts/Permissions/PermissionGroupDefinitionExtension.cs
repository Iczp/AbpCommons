using System;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.Reflection;

namespace IczpNet.AbpCommons.Permissions;

public static class PermissionGroupDefinitionExtension
{
    public static void AddPermission(this PermissionGroupDefinition group, string[] names, Func<string, LocalizableString> localizableString)
    {
        var permission = group.AddPermission(names[0], localizableString(names[0]));

        for (int i = 1; i < names.Length; i++)
        {
            permission.AddChild(names[i], localizableString(names[i]));
        }
    }

    public static void AddPermission<T>(this PermissionGroupDefinition group, Func<string, LocalizableString> localizableString)
    {
        AddPermission(group, typeof(T), localizableString);
    }

    public static void AddPermission(this PermissionGroupDefinition group, Type type, Func<string, LocalizableString> localizableString)
    {
        var names = ReflectionHelper.GetPublicConstantsRecursively(type);

        AddPermission(group, names, localizableString);
    }

    public static void AddPermissions(this PermissionGroupDefinition group, Type rootPermissionType, Func<string, LocalizableString> localizableString)
    {
        foreach (var nestedType in rootPermissionType.GetNestedTypes())
        {
            AddPermission(group, nestedType, localizableString);
        }
    }
    public static void AddPermissions<T>(this PermissionGroupDefinition group, Func<string, LocalizableString> localizableString)
    {
        AddPermissions(group, typeof(T), localizableString);
    }
}
