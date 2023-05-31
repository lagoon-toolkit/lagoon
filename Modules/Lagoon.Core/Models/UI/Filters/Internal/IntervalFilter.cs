namespace Lagoon.UI.Components;

/// <summary>
/// Number filter expressions.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TIntervalFilterItem">The IntervalFilterItem type.</typeparam>
public abstract class IntervalFilter<TValue, TIntervalFilterItem> : Filter<TValue, TIntervalFilterItem>
    where TIntervalFilterItem : IntervalFilterItem<TValue>, new()
{

    #region methods

    /// <summary>
    /// Get the list of the (first...) "In list" filter.
    /// </summary>
    /// <returns>The list of the (first...) "In list" filter.<c>null</c> id not found.</returns>
    public FilterRange<TValue> GetFirstRange()
    {
        return Values.FirstOrDefault(x => x.Range is not null)?.Range;
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="includeMinimum">Indicate if the minimum value is incled in the result.</param>
    /// <param name="maximumValue"></param>
    /// <param name="includeMaximum">Indicate if the maximum value is incled in the result.</param>
    public void AddBetween(TValue minimumValue, bool includeMinimum, TValue maximumValue, bool includeMaximum)
    {
        if (minimumValue is null)
        {
            AddLessThan(maximumValue, includeMaximum);
        }
        else if (maximumValue is null)
        {
            AddGreaterThan(minimumValue, includeMinimum);
        }
        else
        {
            Values.Add(new TIntervalFilterItem()
            {
                Range = new FilterRange<TValue>
                {
                    Minimum = new(minimumValue, !includeMinimum),
                    Maximum = new(maximumValue, !includeMaximum)
                }
            });
        }
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="range">The range.</param>
    public void AddBetween(FilterRange<TValue> range)
    {
        if (range is not null && !range.IsEmpty)
        {
            Values.Add(new TIntervalFilterItem()
            {
                Range = range
            });
        }
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="orEqual">Indicate if the minimum value is incled in the result.</param>
    public void AddGreaterThan(TValue minimumValue, bool orEqual)
    {
        if (minimumValue is not null)
        {
            Values.Add(new TIntervalFilterItem()
            {
                Range = new FilterRange<TValue>
                {
                    Minimum = new(minimumValue, !orEqual),
                }
            });
        }
    }

    /// <summary>
    /// Add a between condition.
    /// </summary>
    /// <param name="maximumValue"></param>
    /// <param name="orEqual">Indicate if the maximum value is incled in the result.</param>
    public void AddLessThan(TValue maximumValue, bool orEqual)
    {
        if (maximumValue is not null)
        {
            Values.Add(new TIntervalFilterItem()
            {
                Range = new FilterRange<TValue>
                {
                    Maximum = new(maximumValue, !orEqual)
                }
            });
        }
    }

    #endregion

}
