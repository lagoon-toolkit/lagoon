namespace Lagoon.UI.Components;


/// <summary>
/// Filter parameter
/// </summary>
/// <typeparam name="TValue">A number type.</typeparam>
public class IntervalFilterItem<TValue> : FilterItem<TValue>
{

    #region properties
    
    /// <summary>
    /// Range of values to include in the filtred list.
    /// </summary>
    [JsonPropertyName("rng")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public FilterRange<TValue> Range { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    protected override IEnumerable<LambdaExpression> GetWhereIncludeExpressions(FilterWhereContext context)
    {
        if (Range is not null)
        {
            if (Range.HasMinimum)
            {
                yield return GetWhereBetweenFunction(Range.Minimum, true);
            }
            if (Range.HasMaximum)
            {
                yield return GetWhereBetweenFunction(Range.Maximum, false);
            }
        }
        foreach (LambdaExpression expression in base.GetWhereIncludeExpressions(context))
        {
            yield return expression;
        }
    }

    /// <summary>
    /// Get the expression representing the limit.
    /// </summary>
    /// <param name="limit">The range limit.</param>
    /// <param name="isMinimum">Indicate if it's the lower boundery; else it's the higher boundery.</param>
    /// <returns>The expression representing the limit.</returns>
    private static Expression<Func<TValue, bool>> GetWhereBetweenFunction(FilterRangeLimit<TValue> limit, bool isMinimum)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TValue), "x");
        ConstantExpression value = Expression.Constant(limit.Value, typeof(TValue));
        BinaryExpression body;
        if (isMinimum)
        {
            if (limit.Excluded)
            {
                // x > Range.Maximum.Value
                body = Expression.GreaterThan(parameter, value);
            }
            else
            {
                // x >= Range.Maximum.Value
                body = Expression.GreaterThanOrEqual(parameter, value);
            }
        }
        else
        {
            if (limit.Excluded)
            {
                // x < Range.Maximum.Value
                body = Expression.LessThan(parameter, value);
            }
            else
            {
                // x <= Range.Maximum.Value
                body = Expression.LessThanOrEqual(parameter, value);
            }
        }
        return Expression.Lambda<Func<TValue, bool>>(body, parameter);
    }

    #endregion

}
