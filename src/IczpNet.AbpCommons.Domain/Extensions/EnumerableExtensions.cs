using NUglify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// EnumerableExtensions
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 从集合中的选出K个元素组合数
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> sequences, int k)
    {
        return k == 0 ? new[] { new T[0] } : sequences.SelectMany((e, i) => sequences.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }
    /// <summary>
    /// 求集合的笛卡尔积
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Cartesian<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> tempProduct = new[] { Enumerable.Empty<T>() };
        return sequences.Aggregate(tempProduct, (accumulator, sequence) =>
                                         from accseq in accumulator
                                         from item in sequence
                                         select accseq.Concat(new[] { item })
                                   );
    }

    private static void aa()
    {

        List<List<string>> items = new List<List<string>>()
        {
            new List<string>(){ "a1","a2","a3"},
            new List<string>(){ "b4","b5"},
            new List<string>(){ "c6" }
        };

        foreach (var item in items.Cartesian())
        {
            //console.WriteLine(string.Join(",", item));
        }
    }
    /// <summary>
    /// if null to empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }
    /// <summary>
    /// if null to ToList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<T> NullToList<T>(this IEnumerable<T> source)
    {
        return source.NullToEmpty().ToList();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> list)
    {
        return list.NullToEmpty().Any();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return list.NullToEmpty().Any(predicate);
    }

    /// <summary>
    /// 查找列表中的重复项，基于提供的键选择器。
    /// </summary>
    /// <typeparam name="T">项的类型。</typeparam>
    /// <typeparam name="TKey">键的类型。</typeparam>
    /// <param name="itemList">包含项的列表。</param>
    /// <param name="keySelector">用于选择键的函数。</param>
    /// <returns>包含重复项的列表。</returns>
    public static List<T> FindDuplicateItems<T, TKey>(this IEnumerable<T> itemList, Func<T, TKey> keySelector)
    {
        // 使用LINQ的GroupBy和Where来找出重复的项
        return itemList
            .GroupBy(keySelector)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Skip(1)) // 只返回重复项中的额外实例，保留第一个实例以避免误报
            .ToList();
    }
}
