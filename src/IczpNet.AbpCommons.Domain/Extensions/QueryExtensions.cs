using System;
using System.Linq;
using System.Linq.Expressions;

namespace IczpNet.AbpCommons.Extensions;

/// <summary>
/// QueryExtensions
/// </summary>
public static class QueryExtensions
{
    private const string SORT_DIRECTION_DESC = " DESC";
    /// <summary>
    /// SortBy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sortExpression"></param>
    /// <returns></returns>
    public static IQueryable<T> SortingBy<T>(this IQueryable<T> source, string sortExpression) where T : class
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (string.IsNullOrWhiteSpace(sortExpression))
        {
            return source;
        }

        sortExpression = sortExpression.Trim();
        bool flag = false;
        if (sortExpression.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
        {
            flag = true;
            int length = sortExpression.Length - " DESC".Length;
            sortExpression = sortExpression.Substring(0, length).Trim();
        }

        if (string.IsNullOrEmpty(sortExpression))
        {
            return source;
        }

        ParameterExpression parameterExpression = Expression.Parameter(source.ElementType, string.Empty);
        MemberExpression memberExpression = null;
        string[] array = sortExpression.Split('.');
        string[] array2 = array;
        foreach (string propertyName in array2)
        {
            memberExpression = ((memberExpression != null) ? Expression.Property(memberExpression, propertyName) : Expression.Property(parameterExpression, propertyName));
        }

        LambdaExpression expression = Expression.Lambda(memberExpression, parameterExpression);
        string methodName = flag ? "OrderByDescending" : "OrderBy";
        Expression expression2 = Expression.Call(typeof(Queryable), methodName, new Type[2]
        {
            source.ElementType,
            memberExpression.Type
        }, source.Expression, Expression.Quote(expression));
        return (IQueryable<T>)source.Provider.CreateQuery(expression2);
    }
    
}
