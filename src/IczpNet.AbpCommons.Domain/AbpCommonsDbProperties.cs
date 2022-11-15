namespace IczpNet.AbpCommons;

public static class AbpCommonsDbProperties
{
    public static string DbTablePrefix { get; set; } = "AbpCommons";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "AbpCommons";
}
