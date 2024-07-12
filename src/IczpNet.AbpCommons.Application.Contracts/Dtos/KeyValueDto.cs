namespace IczpNet.AbpCommons.Dtos;

public class KeyValueDto : KeyValueDto<string>
{

}

public class KeyValueDto<TType> //where T : struct
{
    public virtual TType Key { set; get; }

    public virtual string Value { set; get; }

    public virtual int? Count { set; get; }
}
