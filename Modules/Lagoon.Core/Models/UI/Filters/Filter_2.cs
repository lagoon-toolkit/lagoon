namespace Lagoon.UI.Components;


/// <summary>
/// Gridview columns filters
/// </summary>
public abstract class Filter<TValue, TFilterItem> : Filter<TValue>
    where TFilterItem : FilterItem<TValue>, new()
{

    #region fields

    // Condition list of lines to include.
    private List<TFilterItem> _andConditions = new();

    #endregion

    #region properties

    /// <summary>
    /// Condition list of lines to include.
    /// </summary>
    public List<TFilterItem> Values => _andConditions;

    /// <summary>
    /// Indicate if the filter have no conditions.
    /// </summary>
    public override bool IsEmpty => _andConditions.Count == 0;

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override void SerializeValues(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, Values, options);
    }

    ///<inheritdoc/>
    internal override void DeserializeValues(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        _andConditions = JsonSerializer.Deserialize<List<TFilterItem>>(ref reader, options);
    }

    /// <summary>
    /// Enumerate all values included in all the in list.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<TValue> EnumerateIncludedValues()
    {
        foreach (TFilterItem item in _andConditions)
        {
            if (item.Values is not null)
            {
                foreach (TValue value in item.Values)
                {
                    yield return value;
                }
            }
        }
    }

    /// <summary>
    /// Set the value used to filter.
    /// </summary>
    /// <param name="value">The value used to filter.</param>
    public override void AddNotEquals(TValue value)
    {
        AddExcludedFromList(new List<TValue>() { value });
    }

    /// <summary>
    /// Set the value used to filter.
    /// </summary>
    /// <param name="value">The value used to filter.</param>
    public override void AddEquals(TValue value)
    {
        AddIncludedInList(new List<TValue>() { value });
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to exclude from the filtred list.</param>
    public override void AddExcludedFromList(params TValue[] values)
    {
        AddExcludedFromList(values.AsEnumerable());
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to exclude from the filtred list.</param>
    public override void AddExcludedFromList(IEnumerable<TValue> values)
    {
        AddFromList(true, values);
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public override void AddIncludedInList(params TValue[] values)
    {
        AddIncludedInList(values.AsEnumerable());
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public override void AddIncludedInList(IEnumerable<TValue> values)
    {
        AddFromList(false, values);
    }

    /// <summary>
    /// Add a "value is in list" condition.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public override void AddFromList(bool exclude, params TValue[] values)
    {
        AddFromList(exclude, values.AsEnumerable());
    }

    /// <summary>
    /// Add a "value is in list" condition.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public override void AddFromList(bool exclude, IEnumerable<TValue> values)
    {
        List<TValue> list = values?.ToList();
        if (list is not null && list.Count > 0)
        {
            TFilterItem filterValueItem = new() { Values = list, Exclude = exclude };
            _andConditions.Add(filterValueItem);
        }
    }

    /// <summary>
    /// Get all filter expressions. 
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>All filter expressions.</returns>
    public override IEnumerable<LambdaExpression> GetAndExpressions(FilterWhereContext context)
    {
        foreach (TFilterItem andCondition in _andConditions)
        {
            foreach (LambdaExpression lambda in andCondition.GetWhereExpressions(context))
            {
                yield return lambda;
            }
        }
    }

    #endregion

}
