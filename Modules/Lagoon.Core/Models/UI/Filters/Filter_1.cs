using Lagoon.Helpers.Expressions;

namespace Lagoon.UI.Components;


/// <summary>
/// Gridview columns filters
/// </summary>
public abstract class Filter<TValue> : Filter
{

    /// <summary>
    /// Set the value used to filter.
    /// </summary>
    /// <param name="value">The value used to filter.</param>
    public abstract void AddNotEquals(TValue value);

    /// <summary>
    /// Set the value used to filter.
    /// </summary>
    /// <param name="value">The value used to filter.</param>
    public abstract void AddEquals(TValue value);

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to exclude from the filtred list.</param>
    public abstract void AddExcludedFromList(params TValue[] values);

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to exclude from the filtred list.</param>
    public abstract void AddExcludedFromList(IEnumerable<TValue> values);

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public abstract void AddIncludedInList(params TValue[] values);

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public abstract void AddIncludedInList(IEnumerable<TValue> values);

    /// <summary>
    /// Add a "value is in list" condition.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public abstract void AddFromList(bool exclude, params TValue[] values);

    /// <summary>
    /// Add a "value is in list" condition.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public abstract void AddFromList(bool exclude, IEnumerable<TValue> values);

    /// <summary>
    /// Enumerate all values included in all the in list.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<TValue> EnumerateIncludedValues();

    /// <summary>
    /// Get the where condition.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The where condition.</returns>
    public Func<TValue, bool> GetWhere(FilterWhereContext context)
    {
        Expression where = null;
        ParameterExpression parameter = Expression.Parameter(typeof(TValue), "x");
        foreach (LambdaExpression lambda in GetAndExpressions(context ?? new(false, false)))
        {
            Expression condition = ExpressionParameterReplacer.GetMergeableBody(lambda, parameter);
            where = where is null ? condition : Expression.AndAlso(where, condition);
        }
        if (where is null)
        {
            return null;
        }
        return Expression.Lambda<Func<TValue, bool>>(where, parameter).Compile();
    }

    /// <summary>
    /// Try to convert a filter type to another.
    /// </summary>
    /// <param name="targetFilterType">The target filter type.</param>
    /// <returns>The new filter or null if it's failed or the source filter has no conditions.</returns>
    public override Filter ConvertAs(Type targetFilterType)
    {
        if (targetFilterType is null)
        {
            return null;
        }
        Filter filter = (Filter)Activator.CreateInstance(targetFilterType);
        Type converterType = typeof(FilterConverter<>).MakeGenericType(GetValueType(), filter.GetValueType());
        IFilterConverter converter = (IFilterConverter)Activator.CreateInstance(converterType);
        converter.Copy(this, filter);
        if (filter.IsEmpty)
        {
            return null;
        }
        return filter;
    }


    /// <summary>
    /// Gets the description of the filter.
    /// </summary>
    /// <returns>The description of the filter.</returns>
    public override string ToString()
    {
        return ToString(DefaultFormatValue);
    }

    /// <summary>
    /// Gets the description of the filter.
    /// </summary>
    /// <param name="formatValue">The method to use to format the values.</param>
    /// <returns>The description of the filter.</returns>
    public string ToString(Func<TValue, string> formatValue)
    {
        FilterDescriptionBuilder<TValue> builder = new(formatValue ?? DefaultFormatValue);
        BuildDescription(builder);
        return builder.ToString();
    }

    /// <summary>
    /// Return the default methods to use to format values.
    /// </summary>
    /// <returns>The default methods to use to format values.</returns>
    protected abstract string DefaultFormatValue(TValue value);

    /// <summary>
    /// Get the description of the filter.
    /// </summary>
    /// <returns>The description of the filter.</returns>
    protected abstract void BuildDescription(FilterDescriptionBuilder<TValue> description);

    /// <summary>
    /// Gets the type of the target filtred property.
    /// </summary>
    /// <returns>The type of the target filtred property.</returns>
    public override Type GetValueType()
    {
        return typeof(TValue);
    }

    private interface IFilterConverter
    {
        void Copy(Filter<TValue> sourceFilter, Filter targetFilter);
    }

    private class FilterConverter<TTargetValue> : IFilterConverter
    {
        public void Copy(Filter<TValue> sourceFilter, Filter targetFilter)
        {
            Filter<TTargetValue> filter = (Filter<TTargetValue>)targetFilter;
            filter.AddFromFilter(sourceFilter);
        }
    }

    /// <summary>
    /// Method to copy the filters rules from one filter type to another.
    /// </summary>
    /// <param name="sourceFilter"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void AddFromFilter<T>(Filter<T> sourceFilter)
    {
        List<TValue> list = new();
        foreach (T value in sourceFilter.EnumerateIncludedValues())
        {
            try
            {
                list.Add((TValue)(object)value);
            }
            catch (Exception)
            {
#if DEBUG
                Helpers.Trace.ToConsole(this, $"Fail to copy value !!!");
#endif
            }
        }
        AddIncludedInList(list);
    }

}
