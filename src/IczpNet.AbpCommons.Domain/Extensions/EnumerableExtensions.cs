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
}
