using Volo.Abp.Reflection;

namespace IczpNet.AbpCommons.Permissions;

public class AbpCommonsPermissions
{
    public const string GroupName = "AbpCommons";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AbpCommonsPermissions));
    }
}
