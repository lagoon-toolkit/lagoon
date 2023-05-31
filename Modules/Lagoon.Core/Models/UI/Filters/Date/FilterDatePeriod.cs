namespace Lagoon.UI.Components;


/// <summary>
/// Period of time.
/// </summary>
[JsonConverter(typeof(FilterDatePeriodJsonConverter))]
public class FilterDatePeriod : IComparable<FilterDatePeriod>, IEqualityComparer<FilterDatePeriod>
{

    #region properties

    /// <summary>
    /// Gets the duration of the period with the "kind" property as unit.
    /// </summary>
    public int Duration { get; }

    /// <summary>
    /// Gets the upper excluded limit.
    /// </summary>
    public DateTime ExcludedEnd { get; }

    /// <summary>
    /// Gets the lower included range limit.
    /// </summary>
    public DateTime IncludedStart { get; }

    /// <summary>
    /// Gets the date precision to use.
    /// </summary>
    public FilterDatePeriodKind Kind { get; }

    /// <summary>
    /// The last start period merged.
    /// </summary>
    internal DateTime? LastMergedStart { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="kind">Indicate if the perdiod represent a year, a month or a day.</param>
    /// <param name="includedStart">The included start date.</param>
    /// <param name="duration">Indicate the duration of the period with the "kind" property as unit.</param>
    /// <param name="clean">Indicate if the time part must be removed and if the day and month must be set to 1 according to the kind.</param>
    /// <exception cref="ArgumentOutOfRangeException">The kind must be "Year", "Month" or "Day".</exception>
    public FilterDatePeriod(FilterDatePeriodKind kind, DateTime includedStart, int duration = 1, bool clean = true)
    {
        Kind = kind;
        Duration = duration;
        if (clean)
        {
            // Remove the time part from the date
            includedStart = includedStart.Date;
            // Set the day to the first of the month
            if (kind != FilterDatePeriodKind.Day && includedStart.Day > 1)
            {
                includedStart = includedStart.AddDays(1 - includedStart.Day);
            }
            // Set the day to the first of the year
            if (kind == FilterDatePeriodKind.Year && includedStart.Month > 1)
            {
                includedStart = includedStart.AddMonths(1 - includedStart.Month);
            }
        }
        //// Specify the kind to have correct ODATA queries (Unspecified date types are considered as UTC by ODATA.)
        //if (includedStart.Kind == DateTimeKind.Unspecified)
        //{
        //    includedStart = DateTime.SpecifyKind(includedStart, DateTimeKind.Local);
        //}
        // Save the boundaries
        IncludedStart = includedStart;
        try
        {
            ExcludedEnd = AddDuration(includedStart, duration);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Can't be after 31/12/9999, handle "The added or subtracted value results in an un-representable DateTime."
            ExcludedEnd = DateTime.MaxValue.Date;
        }
        if (duration > 1)
        {
            LastMergedStart = AddDuration(ExcludedEnd, -1);
        }
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="period">The source period defining the kind and the start date.</param>
    /// <param name="lastMergedStart">The last merged start.</param>
    /// <param name="excludedEnd">The end boundary.</param>
    /// <param name="duration">Indicate the duration of the period with the "kind" property as unit.</param>
    internal FilterDatePeriod(FilterDatePeriod period, DateTime lastMergedStart, DateTime excludedEnd, int duration)
    {
        Kind = period.Kind;
        IncludedStart = period.IncludedStart;
        LastMergedStart = lastMergedStart;
        ExcludedEnd = excludedEnd;
        Duration = duration;
    }

    #endregion

    #region methods


    /// <summary>
    /// Test if the current period is inside another one.
    /// </summary>
    /// <param name="periodList">A period list.</param>
    /// <returns>Return <c>true</c>if the current period is inside another one.</returns>
    public bool IsIncludedIn(IEnumerable<FilterDatePeriod> periodList)
    {
        foreach (FilterDatePeriod period in periodList)
        {
            if (IsIncludedIn(period))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Test if the current period is inside another one.
    /// </summary>
    /// <param name="biggerPeriod">The bigger period.</param>
    /// <returns>Return <c>true</c>if the current period is inside another one.</returns>
    public bool IsIncludedIn(FilterDatePeriod biggerPeriod)
    {
        return IncludedStart >= biggerPeriod.IncludedStart && ExcludedEnd <= biggerPeriod.ExcludedEnd;
    }

    /// <summary>
    /// Compare the start date for the period.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(FilterDatePeriod other)
    {
        int result = DateTime.Compare(IncludedStart, other.IncludedStart);
        if (result != 0)
        {
            return result;
        }
        return DateTime.Compare(ExcludedEnd, other.ExcludedEnd);
    }

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>true if the specified objects are equal; otherwise, false.</returns>
    bool IEqualityComparer<FilterDatePeriod>.Equals(FilterDatePeriod x, FilterDatePeriod y)
    {
        if (x is null)
        {
            return y is null;
        }
        if (y is not null)
        {
            return false;
        }
        if (x.IncludedStart != y.IncludedStart)
        {
            return false;
        }
        if (x.Kind != y.Kind)
        {
            return false;
        }
        return x.Duration == y.Duration;
    }

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    int IEqualityComparer<FilterDatePeriod>.GetHashCode(FilterDatePeriod obj)
    {
        return HashCode.Combine(Kind, IncludedStart, Duration);
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        if (LastMergedStart is null)
        {
            return FormatDate(IncludedStart);
        }
        else
        {
            StringBuilder sb = new(FormatDate(IncludedStart));
            sb.Append('➔');
            sb.Append(FormatDate(LastMergedStart.Value));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Format the date with the good unit.
    /// </summary>
    /// <param name="date">Date to format.</param>
    /// <returns>The formated date.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string FormatDate(DateTime date)
    {
        return Kind switch
        {
            FilterDatePeriodKind.Year => date.Year.ToString(),
            FilterDatePeriodKind.Month => date.ToString("Y"), // YearMonthPattern
            FilterDatePeriodKind.Day => date.ToShortDateString(),
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Return a new date with the added duration by using the kind of the period.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="duration">The duration to add.</param>
    /// <returns>The new date with the added duration.</returns>
    private DateTime AddDuration(DateTime date, int duration)
    {
         return Kind switch
        {
            FilterDatePeriodKind.Year => date.AddYears(duration),
            FilterDatePeriodKind.Month => date.AddMonths(duration),
            FilterDatePeriodKind.Day => date.AddDays(duration),
            _ => throw new InvalidOperationException("The FilterDatePeriodKind.{Kind} is not handled.")
        };
    }

    #endregion
}

