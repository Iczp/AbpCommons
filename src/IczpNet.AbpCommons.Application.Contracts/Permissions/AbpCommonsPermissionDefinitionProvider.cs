using IczpNet.AbpCommons.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IczpNet.AbpCommons.Permissions;

public class AbpCommonsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AbpCommonsPermissions.GroupName, L("Permission:AbpCommons"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpCommonsResource>(name);
    }
}
