using System;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// IsEmptyExtensions
/// </summary>
public static class IsEmptyExtensions
{
    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this Guid? value)
    {
        if (value == null)
            return true;
        return Guid.Empty.Equals(value);
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this Guid value)
    {
        if (value == Guid.Empty)
            return true;
        return false;
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <param name="value">值</param>
    public static bool IsEmpty(this object value)
    {
        if (value != null && !string.IsNullOrEmpty(value.ToString()))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
