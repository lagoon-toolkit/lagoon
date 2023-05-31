namespace Lagoon.UI.Components;


/// <summary>
/// Date filter expressions.
/// </summary>
/// <typeparam name="TDateTime">DateTime or DateTime? types.</typeparam>
public class DateFilter<TDateTime> : IntervalFilter<TDateTime, DateFilterItem<TDateTime>>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public DateFilter()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public DateFilter(params TDateTime[] values) : this(values.AsEnumerable()) {}

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public DateFilter(IEnumerable<TDateTime> list)
    {
        // TODO Create a period list from a date list
        throw new NotImplementedException();
    }

    /// <summary>
    /// New instance. Add the filter as a new period selection.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="periods">List of period.</param>
    public DateFilter(bool exclude, IEnumerable<FilterDatePeriod> periods)
    {
        AddFromPeriods(exclude, periods);
    }
    /// <summary>
    /// New instance. Add a between condition.
    /// </summary>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="includeMinimum">Indicate if the minimum value is incled in the result.</param>
    /// <param name="maximumValue"></param>
    /// <param name="includeMaximum">Indicate if the maximum value is incled in the result.</param>
    public DateFilter(TDateTime minimumValue, bool includeMinimum, TDateTime maximumValue, bool includeMaximum)
    {
        AddBetween(minimumValue, includeMinimum, maximumValue, includeMaximum);
    }

    #endregion

    #region methods

    /// <summary>
    /// Get the list of the (first...) "In list" filter.
    /// </summary>
    /// <returns>The list of the (first...) "In list" filter.<c>null</c> id not found.</returns>
    public FilterDatePeriodCollection GetFirstPeriodCollection()
    {
        return Values.FirstOrDefault(x => x.Periods is not null)?.Periods;
    }

    /// <summary>
    /// Add a "value must not be is in list" condition.
    /// </summary>
    /// <param name="periods">The list of periods to exclude in the filtred list.</param>
    public void AddExcludedFromPeriods(IEnumerable<FilterDatePeriod> periods)
    {
        AddFromPeriods(true, periods);
    }

    /// <summary>
    /// Add a "value is in list" condition.
    /// </summary>
    /// <param name="periods">The list of periods to include in the filtred list.</param>
    public void AddIncludedInPeriods(IEnumerable<FilterDatePeriod> periods)
    {
        AddFromPeriods(false, periods);
    }

    /// <summary>
    /// Add the filter as a new period selection.
    /// </summary>
    /// <param name="exclude">Indicate if the values in the list must be excluded; else the value in the list must be included.</param>
    /// <param name="periods">List of period.</param>
    public void AddFromPeriods(bool exclude, IEnumerable<FilterDatePeriod> periods)
    {
        var item = new DateFilterItem<TDateTime>()
        {
            Periods = new(periods)
        };
        if(item.Periods.Count > 0)
        {
            item.Exclude = exclude;
            Values.Add(item);
        }
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TDateTime value)
    {
        return value switch
        {
            null => null,
            DateTime dt => dt.ToShortDateString(),
            DateTimeOffset dto => dto.DateTime.ToShortDateString(),
            _ => value.ToString()
        };
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<TDateTime> description)
    {
        foreach (DateFilterItem<TDateTime> filter in Values)
        {
            // Periods
            if (filter.Periods is not null)
            {
                foreach (FilterDatePeriod period in filter.Periods)
                {
                    AppendPeriod(description, period);
                }
            }
            // Ranges
            if (filter.Range is not null)
            {
                bool hasMinValue = filter.Range.Minimum is not null;
                if(hasMinValue)
                {                        
                    description.AppendSeparator();
                    description.AppendValue(filter.Range.Minimum.Value);
                }
                if (filter.Range.Maximum is not null)
                {
                    if(hasMinValue)
                    {
                        description.AppendBetweenSeparator();
                    }
                    else
                    {
                        description.AppendSeparator();
                    }
                    description.AppendValue(filter.Range.Maximum.Value);
                }
            }
            // Included values
            if (filter.Values is not null)
            {
                foreach (TDateTime date in filter.Values)
                {
                    description.AppendValue(date);
                }
            }
        }
    }

    /// <summary>
    /// Add a period to the description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="period">The period.</param>
    /// <param name="addSeparator">Add a separator before (if the description is not empty).</param>
    private static void AppendPeriod(FilterDescriptionBuilder<TDateTime> description, FilterDatePeriod period, bool addSeparator = true)
    {
        if (period is null)
        {
            description.AppendValue(default, addSeparator);
        }
        else
        {
            description.AppendSeparator(addSeparator);
            if (period.LastMergedStart is null)
            {
                AppendPeriodDate(description, period.Kind, period.IncludedStart);
            }
            else
            {
                AppendPeriodDate(description, period.Kind, period.IncludedStart);
                description.AppendBetweenSeparator();
                AppendPeriodDate(description, period.Kind, period.LastMergedStart.Value);
            }
        }
    }

    /// <summary>
    /// Format the date with the good unit.
    /// </summary>
    /// <param name="description">The description builder.</param>
    /// <param name="kind">The date unit.</param>
    /// <param name="date">Date to format.</param>
    /// <returns>The formated date.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static void AppendPeriodDate(FilterDescriptionBuilder<TDateTime> description, FilterDatePeriodKind kind, DateTime date)
    {
        switch (kind)
        {
            case FilterDatePeriodKind.Year:
                description.Append(date.ToString("yyyy"));
                break;
            case FilterDatePeriodKind.Month:
                description.Append(date.ToString("Y"));
                break;
            case FilterDatePeriodKind.Day:
                description.Append(date.ToShortDateString());
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    #endregion

}
