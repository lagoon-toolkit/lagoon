namespace Lagoon.UI.Components;

/// <summary>
/// Range of values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class FilterRange<TValue>
{

    #region properties

    /// <summary>
    /// Minumum value of the range.
    /// </summary>
    [JsonPropertyName("min")]
    public FilterRangeLimit<TValue> Minimum { get; set; }

    /// <summary>
    /// Maximum value of the range.
    /// </summary>
    [JsonPropertyName("max")]
    public FilterRangeLimit<TValue> Maximum { get; set; }

    /// <summary>
    /// Gets if the range is empty.
    /// </summary>
    public bool IsEmpty => !HasMinimum && !HasMaximum;

    /// <summary>
    /// Gets if the range has a maximum value.
    /// </summary>
    public bool HasMaximum => Maximum is not null && Maximum.Value is not null;

    /// <summary>
    /// Gets if the range has a minimum value.
    /// </summary>
    public bool HasMinimum => Minimum is not null &&  Minimum.Value is not null;

    #endregion

}
