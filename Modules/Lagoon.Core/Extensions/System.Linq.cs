namespace System.Linq;

/// <summary>
/// Helper extension methods
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// Apply filter to the source.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The list of items.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The new enumerable with the applied filter.</returns>
    public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> source, Filter<T> filter, FilterWhereContext context)
    {
        Func<T, bool> method = filter.GetWhere(context);
        return method is null ? source : source.Where(method);
    }

    /// <summary>
    /// Build a new query by adding the model filter conditions.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="modelFilter">The model filter.</param>
    /// <returns>The new query.</returns>
    public static IQueryable<T> ApplyModelFilter<T>(this IQueryable<T> query, ModelFilter<T> modelFilter)
    {
        if (modelFilter is not null && !modelFilter.IsEmpty)
        {
            query = query.Where(modelFilter.GetWhereExpression());
        }
        return query;
    }

}
