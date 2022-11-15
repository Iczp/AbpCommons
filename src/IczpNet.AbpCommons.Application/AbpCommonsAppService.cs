using IczpNet.AbpCommons.Localization;
using Volo.Abp.Application.Services;

namespace IczpNet.AbpCommons;

public abstract class AbpCommonsAppService : ApplicationService
{
    protected AbpCommonsAppService()
    {
        LocalizationResource = typeof(AbpCommonsResource);
        ObjectMapperContext = typeof(AbpCommonsApplicationModule);
    }
}
