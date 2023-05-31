namespace Lagoon.UI.Components;

/// <summary>
/// A Value that limit a range.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class FilterRangeLimit<TValue>
{

    #region properties

    /// <summary>
    /// Get or set the value of the limit.
    /// </summary>
    [JsonPropertyName("val")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TValue Value { get; set; }

    /// <summary>
    /// Gets or sets if the value must be included in the range.
    /// </summary>
    [JsonPropertyName("out")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Excluded { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public FilterRangeLimit()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="value">The value.</param>
    public FilterRangeLimit(TValue value)
    {
        Value = value;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="excluded">The value is exclude from the range.</param>
    public FilterRangeLimit(TValue value, bool excluded)
    {
        Value = value;
        Excluded = excluded;
    }

    #endregion

}
