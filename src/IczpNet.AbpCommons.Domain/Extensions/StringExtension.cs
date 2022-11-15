namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// 枚举工具类(Iczp.Net)
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 字符限制最大长度
    /// </summary>
    /// <param name="input"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string MaxLength(this string input, int maxLength)
    {
        return input.Length > maxLength ? input.Substring(0, maxLength) : input;
    }
}
