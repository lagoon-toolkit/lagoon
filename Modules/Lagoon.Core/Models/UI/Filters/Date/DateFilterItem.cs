namespace Lagoon.UI.Components;


/// <summary>
/// Filter parameter
/// </summary>
/// <typeparam name="TValue">DateTime or Nullable{DateTime}</typeparam>
public sealed class DateFilterItem<TValue> : IntervalFilterItem<TValue>
{

    #region properties

    /// <summary>
    /// List of values to include in the filtred list.
    /// </summary>
    [JsonPropertyName("periods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FilterDatePeriodCollection Periods { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    protected override IEnumerable<LambdaExpression> GetWhereIncludeExpressions(FilterWhereContext context)
    {
        if (Periods is not null && Periods.Count != 0)
        {
            yield return GetWhereInPeriods(context);
        }
        foreach (LambdaExpression expression in base.GetWhereIncludeExpressions(context))
        {
            yield return expression;
        }
    }

    /// <summary>
    /// The expression representing text contains condition.
    /// </summary>
    /// <returns>he expression representing text contains condition.</returns>
    private Expression<Func<TValue, bool>> GetWhereInPeriods(FilterWhereContext context)
    {
        bool handleNull = Nullable.GetUnderlyingType(typeof(TValue)) != null;
        bool includeNull = false;
        Expression where = null;
        Expression condition;
        ParameterExpression xParameter = Expression.Parameter(typeof(TValue), "x");
        Expression xDateTimeParameterValue;
        Expression xDateTimeParameterHasValue;
        if (handleNull)
        {
            xDateTimeParameterValue = Expression.Property(xParameter, "Value");
            xDateTimeParameterHasValue = Expression.Property(xParameter, "HasValue");
        }
        else
        {
            xDateTimeParameterValue = xParameter;
            xDateTimeParameterHasValue = null;
        }
        foreach (FilterDatePeriod period in Periods)
        {
            if (period is null)
            {
                includeNull = true;
            }
            else
            {
                // x => x >= period.IncludedStart && x < period.ExcludedEnd
                condition = Expression.AndAlso(
                    Expression.GreaterThanOrEqual(xDateTimeParameterValue, Expression.Constant(period.IncludedStart)),
                    Expression.LessThan(xDateTimeParameterValue, Expression.Constant(period.ExcludedEnd)));
                if (handleNull && !context.TargetEF)
                {
                    condition = Expression.AndAlso(xDateTimeParameterHasValue, condition);
                }
                // Add other inclusion
                where = where is null ? condition : Expression.OrElse(where, condition);
            }
        }
        // If they're is a null value and the value is nullable, test the null before other conditions
        if (includeNull && typeof(TValue) == typeof(DateTime?))
        {
            condition = Expression.Equal(xParameter, Expression.Constant(null, typeof(object)));
            where = where is null ? condition : Expression.OrElse(condition, where);
        }
        // Build the final lambda
        return Expression.Lambda<Func<TValue, bool>>(where, xParameter);
    }

    #endregion

}
