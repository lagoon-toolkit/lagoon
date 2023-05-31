namespace Lagoon.Helpers;

/// <summary>
/// Collection of filters to be applied to the model.
/// </summary>
/// <typeparam name="TModel">The model.</typeparam>
public class ModelFilter<TModel>
{

    #region fields

    private ParameterExpression _parameter = Expression.Parameter(typeof(TModel), "__x");
    private List<ValueFilter> _filters;
    private readonly FilterWhereContext _context;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    public ModelFilter(FilterWhereContext context)
    {
        _context = context ?? new(false, false);
    }

    #endregion

    #region properties

    /// <summary>
    /// Get filters.
    /// </summary>
    public IEnumerable<ValueFilter> Filters => _filters;

    /// <summary>
    /// Gets if there's no filters.
    /// </summary>
    public bool IsEmpty => _filters is null || !HasAnyCondition();

    #endregion

    #region methods

    /// <summary>.
    /// Add (or replace) a new filter testing if a field contains a text
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="searchMode">The search mode.</param>
    /// <param name="searchedText">The searched text.</param>
    /// <param name="collationType">The type of case and accent sensitivity.</param>
    public void AddTextSearch(Expression<Func<TModel, string>> valueSelector, FilterTextSearchMode searchMode, string searchedText, CollationType collationType)
    {
        AddFilter(valueSelector, new TextFilter(searchMode, searchedText, collationType));
    }

    /// <summary>.
    /// Add (or replace) a new filter testing if a field contains a text
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="searchMode">The search mode.</param>
    /// <param name="searchedText">The searched text.</param>
    public void AddTextSearch(Expression<Func<TModel, string>> valueSelector, FilterTextSearchMode searchMode, string searchedText)
    {
        AddFilter(valueSelector, new TextFilter(searchMode, searchedText));
    }

    /// <summary>
    /// Set the list of values to include in the filtred list.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public void AddIncludedInList<TValue>(Expression<Func<TModel, TValue>> valueSelector, params TValue[] list)
    {
        AddIncludedInList(valueSelector, list.AsEnumerable());
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="values">The list of values to include in the filtred list.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddIncludedInList<TValue>(Expression<Func<TModel, TValue>> valueSelector, IEnumerable<TValue> values)
    {
        Filter<TValue> filter;
        if (valueSelector is Expression<Func<TModel, string>> stringValueSelector)
        {
            filter = AddStringValueFilter(stringValueSelector) as Filter<TValue>;
        }
        else
        {
            Type type = typeof(TValue);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                filter = AddEnumFilter(valueSelector);
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(type);
                filter = typeCode switch
                {
                    TypeCode.Boolean => AddBooleanFilter(valueSelector),
                    TypeCode.DateTime => AddDateFilter(valueSelector),
                    _ => AddNumericFilter(valueSelector)
                };
            }
        }
        filter.AddIncludedInList(values);
    }

    /// <summary>
    /// Add a period condition.
    /// </summary>
    /// <typeparam name="TDateTime">DateTime or Datime? type.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="periods">The list of date periods.</param>
    public void AddFromPeriods<TDateTime>(Expression<Func<TModel, TDateTime>> valueSelector, IEnumerable<FilterDatePeriod> periods)
    {
        AddFilter(valueSelector, new DateFilter<TDateTime>(false, periods));
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="includeMinimum">Indicate if the minimum value is incled in the result.</param>
    /// <param name="maximumValue"></param>
    /// <param name="includeMaximum">Indicate if the maximum value is incled in the result.</param>
    public void AddBetween<TValue>(Expression<Func<TModel, TValue>> valueSelector, TValue minimumValue, bool includeMinimum,
        TValue maximumValue, bool includeMaximum)
    {
        Type type = typeof(TValue);
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type == typeof(DateTime))
        {
            AddFilter(valueSelector, new DateFilter<TValue>(minimumValue, includeMinimum, maximumValue, includeMaximum));
        }
        else
        {
            AddFilter(valueSelector, new NumericFilter<TValue>(minimumValue, includeMinimum, maximumValue, includeMaximum));
        }
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="orEqual">Indicate if the minimum value is incled in the result.</param>
    public void AddGreaterThan<TNumber>(Expression<Func<TModel, TNumber>> valueSelector, TNumber minimumValue, bool orEqual)
    {
        NumericFilter<TNumber> filter = AddNumericFilter(valueSelector);
        filter.AddGreaterThan(minimumValue, orEqual);
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <param name="orEqual">Indicate if the maximum value is incled in the result.</param>
    public void AddLessThan<TNumber>(Expression<Func<TModel, TNumber>> valueSelector, TNumber maximumValue, bool orEqual)
    {
        NumericFilter<TNumber> filter = AddNumericFilter(valueSelector);
        filter.AddLessThan(maximumValue, orEqual);
    }

    /// <summary>
    /// Add a filter on a boolean field.
    /// </summary>
    /// <typeparam name="TBool">The bool or bool? type.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <returns>The new filter for the value.</returns>
    public BooleanFilter<TBool> AddBooleanFilter<TBool>(Expression<Func<TModel, TBool>> valueSelector)
    {
        return AddNewFilter<TBool, BooleanFilter<TBool>>(valueSelector);
    }
    /// <summary>
    /// Add a filter on a date field.
    /// </summary>
    /// <typeparam name="TDateTime">The DateTime or DateTime? type.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <returns>The new filter for the value.</returns>
    public DateFilter<TDateTime> AddDateFilter<TDateTime>(Expression<Func<TModel, TDateTime>> valueSelector)
    {
        return AddNewFilter<TDateTime, DateFilter<TDateTime>>(valueSelector);
    }

    /// <summary>
    /// Add a filter on an enum field.
    /// </summary>
    /// <typeparam name="TEnum">The enum or nullable enum type.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <returns>The new filter for the value.</returns>
    public EnumFilter<TEnum> AddEnumFilter<TEnum>(Expression<Func<TModel, TEnum>> valueSelector)
    {
        return AddNewFilter<TEnum, EnumFilter<TEnum>>(valueSelector);
    }

    /// <summary>
    /// Add a filter on a numeric field.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <returns>The new filter for the value.</returns>
    public NumericFilter<TNumber> AddNumericFilter<TNumber>(Expression<Func<TModel, TNumber>> valueSelector)
    {
        return AddNewFilter<TNumber, NumericFilter<TNumber>>(valueSelector);
    }

    /// <summary>
    /// Add a filter on a text field.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <returns>The new filter for the value.</returns>
    public TextFilter AddStringValueFilter(Expression<Func<TModel, string>> valueSelector)
    {
        return AddNewFilter<string, TextFilter>(valueSelector);
    }

    /// <summary>
    /// Add a new filter to the list.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    private TFilter AddNewFilter<TValue, TFilter>(Expression<Func<TModel, TValue>> valueSelector) where TFilter : Filter<TValue>, new()
    {
        TFilter filter = new();
        AddFilter(valueSelector, filter);
        return filter;
    }

    /// <summary>
    /// Add a new filter for a selection in the filter for the model.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <typeparam name="TFilter">The type of filter.</typeparam>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="filter">The filter.</param>
    public void AddFilter<TValue, TFilter>(Expression<Func<TModel, TValue>> valueSelector, TFilter filter)
        where TFilter : Filter<TValue>
    {
        AddFilterUnsafe(valueSelector, filter);
    }

    /// <summary>
    /// Add a new filter for a selection in the filter for the model.
    /// </summary>
    /// <param name="valueSelector">The selector for the value.</param>
    /// <param name="filter">The filter.</param>
    /// <remarks>The type compatibility is not checked between the lambda expression and the filter type.</remarks>
    public void AddFilterUnsafe(LambdaExpression valueSelector, Filter filter)
    {
        _filters ??= new();
        _filters.Add(new ValueFilter(valueSelector, filter));
    }

    /// <summary>
    /// Merge all value conditions.
    /// </summary>
    /// <returns>An expression with all conditions. Return <c>null</c> if there is no conditions.</returns>
    public Func<TModel, bool> GetWhere()
    {
        return GetWhereExpression().Compile();
    }

    /// <summary>
    /// Gets if the filter contains any condition.
    /// </summary>
    /// <returns><c>true</c> if the filter contains any condition.</returns>
    private bool HasAnyCondition()
    {
        foreach (ValueFilter filter in _filters)
        {
            if (filter.GetWhereConditionExpressions(_parameter, _context).Any())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Merge all value conditions.
    /// </summary>
    /// <returns>An expression with all conditions. Return <c>null</c> if there is no conditions.</returns>
    public Expression<Func<TModel, bool>> GetWhereExpression()
    {
        Expression where = null;
        foreach (ValueFilter filter in _filters)
        {
            foreach (Expression condition in filter.GetWhereConditionExpressions(_parameter, _context))
            {
                where = where is null ? condition : Expression.AndAlso(where, condition);
            }
        }
        if (where is null)
        {
            return null;
        }
        return Expression.Lambda<Func<TModel, bool>>(where, _parameter);
    }

#if DEBUG

    /// <summary>
    /// Serialize filter values to JSON array (test purpose).
    /// </summary>
    /// <returns>The Json array.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string FiltersToJson()
    {
        StringBuilder sb = new("[");
        foreach (ValueFilter filter in Filters)
        {
            if (sb.Length > 1)
            {
                sb.Append(',');
            }
            sb.Append(JsonSerializer.Serialize(filter.Filter));
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Return the list of filter types (test purpose).
    /// </summary>
    /// <returns>The list of filter types (test purpose).</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string GetFilterTypes()
    {
        StringBuilder sb = new();
        foreach (ValueFilter filter in Filters)
        {
            sb.AppendLine(filter.Filter.GetType().FriendlyName());
        }
        return sb.ToString();
    }

#endif

    #endregion
}
