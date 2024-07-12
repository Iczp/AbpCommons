using System.Collections.Generic;

namespace IczpNet.AbpCommons.Dtos;

public class IdListInput<Tkey>
{
    public List<Tkey> IdList { get; set; }
}
