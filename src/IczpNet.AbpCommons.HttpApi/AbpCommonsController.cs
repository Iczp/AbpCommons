using IczpNet.AbpCommons.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IczpNet.AbpCommons;

public abstract class AbpCommonsController : AbpControllerBase
{
    protected AbpCommonsController()
    {
        LocalizationResource = typeof(AbpCommonsResource);
    }
}
