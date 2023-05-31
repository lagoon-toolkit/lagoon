namespace Lagoon.UI.Components;

/// <summary>
/// A collection of date periods.
/// </summary>
public class FilterDatePeriodCollection : List<FilterDatePeriod>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public FilterDatePeriodCollection()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="periods">The period list.</param>
    public FilterDatePeriodCollection(IEnumerable<FilterDatePeriod> periods) : base(ConsolidateList(periods))
    {
    }

    #endregion

    #region methods

    /// <summary>
    /// Indicate if the period is include for the specified kind.
    /// </summary>
    /// <param name="period">The period to find.</param>
    /// <returns><c>true</c>if the period is include for the specified kind.</returns>
    public bool ContainsForKind(FilterDatePeriod period)
    {
        if (period is null)
        {
            return Contains(null);
        }
        foreach (FilterDatePeriod p in this)
        {
            if (p is not null && p.Kind == period.Kind && period.IncludedStart >= p.IncludedStart && period.ExcludedEnd <= p.ExcludedEnd)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Create a simplified list of periods.
    /// </summary>
    /// <param name="periods">Period list.</param>
    /// <returns>A simplified list of periods.</returns>
    public static IEnumerable<FilterDatePeriod> ConsolidateList(IEnumerable<FilterDatePeriod> periods)
    {
        List<List<FilterDatePeriod>> list = new() { new(), new(), new() };
        List<FilterDatePeriod> source = new(periods);
        FilterDatePeriod last = null;
        bool hasNull = false;
        // Sort by kind first and by date
        source.Sort(ConsolidateCompare);
        // Split list and merge consecutive values
        foreach (FilterDatePeriod period in source)
        {
            if(period is null)
            {
                hasNull = true;
            }
            else
            {
                if (last is not null && last.Kind == period.Kind && period.IncludedStart <= last.ExcludedEnd)
                {
                    if (period.ExcludedEnd > last.ExcludedEnd)
                    {
                        // replace the last period with a consecutive one
                        last = new(last, period.IncludedStart, period.ExcludedEnd, last.Duration + period.Duration);
                        list[(int)last.Kind][^1] = last;
                    }
                }
                else
                {
                    last = period;
                    if (last is not null)
                    {
                        list[(int)last.Kind].Add(last);
                    }
                }
            }
        }
        // Remove period that are already included in an higher list
        RemoveIntersections(list[(int)FilterDatePeriodKind.Month], list[(int)FilterDatePeriodKind.Year]);
        RemoveIntersections(list[(int)FilterDatePeriodKind.Day], list[(int)FilterDatePeriodKind.Year]);
        RemoveIntersections(list[(int)FilterDatePeriodKind.Day], list[(int)FilterDatePeriodKind.Month]);
        // Add the null value
        if(hasNull)
        {
            list.Insert(0, new() { null });
        }
        // Return the concatenation of differents kind of lists
        return list.SelectMany(x => x).OrderBy(x => x);
    }

    /// <summary>
    /// Remove from the list the period already covered by the bigger list.
    /// </summary>
    /// <param name="list">The list to clean.</param>
    /// <param name="biggerList">The biggest list.</param>
    private static void RemoveIntersections(List<FilterDatePeriod> list, List<FilterDatePeriod> biggerList)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].IsIncludedIn(biggerList))
            {
                list.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Compare period in order to consolidate list.
    /// </summary>
    /// <param name="x">One period.</param>
    /// <param name="y">Another one perdiod.</param>
    /// <returns></returns>
    private static int ConsolidateCompare(FilterDatePeriod x, FilterDatePeriod y)
    {
        if (x is null || y is null)
        {
            if(ReferenceEquals(x, y))
            {
                return 0;
            }
            else
            {
                return y is null ? 1 : -1;
            }
        }
        int result = x.Kind - y.Kind;
        if (result != 0)
        {
            return result;
        }
        result = DateTime.Compare(x.IncludedStart, y.IncludedStart);
        if (result != 0)
        {
            return result;
        }
        return DateTime.Compare(x.ExcludedEnd, y.ExcludedEnd);
    }

    #endregion

}
