namespace Lagoon.Helpers.Data;

/// <summary>
/// The list of values for the column.
/// </summary>
public class FilterPreviewValues<TValue>
{

    /// <summary>
    /// Indicate that only partial data have been returned. (Timeout)
    /// </summary>
    public bool IsPartial { get; set; }

    /// <summary>
    /// The list of values.
    /// </summary>
    public TValue[] Values { get; set; }

}
