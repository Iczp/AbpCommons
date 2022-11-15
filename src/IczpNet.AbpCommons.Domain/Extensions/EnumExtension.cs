using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// 枚举工具类(Iczp.Net)
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 从枚举中获取Description
    /// 说明：
    /// 单元测试-->通过
    /// </summary>
    /// <param name="enumName">需要获取枚举描述的枚举</param>
    /// <returns>描述内容</returns>
    public static string GetDescription(this Enum enumName)
    {
        var field = enumName.GetType().GetField(enumName.ToString());
        var attributes = field.GetDescriptionAttributes();
        string description;
        if (attributes != null && attributes.Length > 0)
        {
            description = attributes[0].Description;
        }
        else
        {
            description = null;
        }
        return description;
    }

    ///// <summary>
    ///// get enum description by name
    ///// </summary>
    ///// <typeparam name="T">enum type</typeparam>
    ///// <param name="enumItemName">the enum name</param>
    ///// <returns></returns>
    //public static string GetDescription<T>(this T enumItemName)
    //{
    //    FieldInfo field = enumItemName.GetType().GetField(enumItemName.ToString());

    //    DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(
    //        typeof(DescriptionAttribute), false);

    //    if (attributes != null && attributes.Length > 0)
    //    {
    //        return attributes[0].Description;
    //    }
    //    else
    //    {
    //        return enumItemName.ToString();
    //    }
    //}
    /// <summary>
    /// 获取字段Description
    /// </summary>
    /// <param name="fieldInfo">FieldInfo</param>
    /// <returns>DescriptionAttribute[] </returns>
    public static DescriptionAttribute[] GetDescriptionAttributes(this FieldInfo fieldInfo)
    {
        if (fieldInfo != null)
        {
            return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        }
        return null;
    }
    /// <summary>
    /// 根据Description获取枚举
    /// 说明：
    /// 单元测试-->通过
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="description">枚举描述</param>
    /// <returns>枚举</returns>
    public static T AsEnum<T>(this string description)
    {
        var type = typeof(T);
        foreach (var field in type.GetFields())
        {
            var attributes = field.GetDescriptionAttributes();
            if (attributes != null && attributes.Length > 0)
            {
                if (attributes[0].Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            else
            {
                if (field.Name == description)
                {
                    return (T)field.GetValue(null);
                }
            }
        }
        throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
    }
    /// <summary>
    /// 将枚举转换为ArrayList
    /// 说明：
    /// 若不是枚举类型，则返回NULL
    /// 单元测试-->通过
    /// </summary>
    /// <param name="type">枚举类型</param>
    /// <returns>ArrayList</returns>
    public static ArrayList ToArrayList(this Type type)
    {
        if (type.IsEnum)
        {
            var _array = new ArrayList();
            var _enumValues = Enum.GetValues(type);
            foreach (Enum value in _enumValues)
            {
                _array.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
            }
            return _array;
        }
        return null;
    }
}
