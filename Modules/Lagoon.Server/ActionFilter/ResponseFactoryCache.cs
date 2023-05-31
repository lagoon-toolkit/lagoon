using Microsoft.AspNetCore.Mvc.Formatters;
using System.Linq.Dynamic.Core;

namespace Lagoon.Server.Controllers;

internal static class ResponseFactoryCache
{

    #region properties

    /// <summary>
    /// Queryable distinct count method
    /// </summary>
    public static MethodInfo QueryableCountDistinct { get; } = GetGenericMethod(_ => CountDistinct<int, int>(default, default));

    /// <summary>
    /// Queryable distinct method
    /// </summary>
    public static MethodInfo QueryableDistinct { get; } = GetGenericMethod(_ => Queryable.Distinct(new List<int>().AsQueryable()));

    /// <summary>
    /// Queryable max method
    /// </summary>
    public static MethodInfo QueryableMax { get; } = GetGenericMethod(_ => Queryable.Max(default, default(Expression<Func<int, int>>)));

    /// <summary>
    /// Queryable min method
    /// </summary>
    public static MethodInfo QueryableMin { get; } = GetGenericMethod(_ => Queryable.Min(default, default(Expression<Func<int, int>>)));

    /// <summary>
    /// Queryable average method
    /// </summary>
    public static MethodInfo QueryableAverage { get; } = GetGenericMethod(_ => Queryable.Average(default, default(Expression<Func<int, int>>)));

    /// <summary>
    /// Queryable sum method
    /// </summary>
    public static MethodInfo QueryableSum { get; } = GetGenericMethod(_ => Queryable.Sum(default, default(Expression<Func<int, int>>)));

    /// <summary>
    /// Queryable select method
    /// </summary>
    public static MethodInfo QueryableSelect { get; } = GetGenericMethod(_ => Queryable.Select(default, default(Expression<Func<int, int>>)));

    /// <summary>
    /// JSON serialize options to limit response size
    /// </summary>
    public static SystemTextJsonOutputFormatter Formatter { get; } = new(new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    });

    #endregion

    #region methods

    /// <summary>
    /// Return a invokable generic method based on an none generic method
    /// </summary>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MethodInfo GetGenericMethod<TReturn>(Expression<Func<object, TReturn>> expression)
    {
        return ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition();
    }

    /// <summary>
    /// Count distinct method
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="query"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static int CountDistinct<TSource, TResult>(IQueryable<TSource> query, Expression<Func<TSource, TResult>> expression)
    {
        return Queryable.Select(query, expression).Distinct().Count();

    }

    #endregion

}
