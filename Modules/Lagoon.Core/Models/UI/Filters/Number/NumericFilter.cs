namespace Lagoon.UI.Components;

/// <summary>
/// Number filter expressions.
/// </summary>
/// <typeparam name="TNumber">A number type.</typeparam>
[JsonConverter(typeof(FilterJsonConverter))]
public class NumericFilter<TNumber> : IntervalFilter<TNumber, NumericFilterItem<TNumber>>
{

    #region fields

    private string _defaultFormat;

    #endregion

    #region properties

    /// <summary>
    /// The default format.
    /// </summary>
    private string DefaultFormat
    {
        get
        {
            _defaultFormat ??= $"N{typeof(TNumber).GetDefaultDecimalDigits():D}";
            return _defaultFormat;
        }
    }

    ///<inheritdoc/>
    public override bool IsNumeric => true;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public NumericFilter()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public NumericFilter(params TNumber[] values)
    {
        AddIncludedInList(values);
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public NumericFilter(IEnumerable<TNumber> list)
    {
        AddIncludedInList(list);
    }

    /// <summary>
    /// New instance. Add a between condition.
    /// </summary>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="includeMinimum">Indicate if the minimum value is incled in the result.</param>
    /// <param name="maximumValue"></param>
    /// <param name="includeMaximum">Indicate if the maximum value is incled in the result.</param>
    public NumericFilter(TNumber minimumValue, bool includeMinimum, TNumber maximumValue, bool includeMaximum)
    {
        AddBetween(minimumValue, includeMinimum, maximumValue, includeMaximum);   
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TNumber value)
    {
        return value switch
        {
            null => null,
            IFormattable numericValue => numericValue.ToString(DefaultFormat, null),
            _ => value.ToString()
        };
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<TNumber> description)
    {
        foreach (NumericFilterItem<TNumber> filter in Values)
        {
            // Ranges
            if (filter.Range is not null)
            {
                description.AppendSeparator();
                bool hasMinValue = filter.Range.Minimum is not null;
                if(hasMinValue && filter.Range.Maximum is not null)
                {
                    // Interval
                    description.Append(filter.Range.Minimum.Excluded ? "(" : "[");
                    description.AppendValue(filter.Range.Minimum.Value, false);
                    description.AppendBetweenSeparator();
                    description.AppendValue(filter.Range.Maximum.Value, false);
                    description.Append(filter.Range.Maximum.Excluded ? ")" : "]");
                }
                else if(hasMinValue)
                {
                    // Only min limit
                    description.Append(filter.Range.Minimum.Excluded ? ">" : "≥");
                    description.AppendValue(filter.Range.Minimum.Value, false);

                }
                else
                {
                    // Only max limit
                    description.Append(filter.Range.Maximum.Excluded ? "<" : "≤");
                    description.AppendValue(filter.Range.Maximum.Value, false);
                }
            }
            // Included values
            if (filter.Values is not null)
            {
                bool separator = true;
                if(filter.Exclude)
                {
                    description.AppendSeparator();
                    description.Append("≠");
                    separator = false;
                }
                foreach(TNumber value in filter.Values)
                {
                    description.AppendValue(value, separator);
                    separator = true;
                }
            }
        }
    }

    #endregion

}
