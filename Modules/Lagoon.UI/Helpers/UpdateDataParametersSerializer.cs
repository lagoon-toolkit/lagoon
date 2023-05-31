namespace Lagoon.Helpers;

/// <summary>
/// Class used to send data to the server.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class UpdateDataParametersSerializer<TItem>
{

    #region properties

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    public TItem Data { get; }

    /// <summary>
    /// Gets field name
    /// </summary>
    /// <value></value>
    public string ColumnKey { get; }

    /// <summary>
    /// Gets new value of the cell
    /// </summary>
    /// <value></value>
    public object Value { get; }

    /// <summary>
    /// Gets previous value of the cell
    /// </summary>
    /// <value></value>
    public object PreviousValue { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance to update an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="columnKey">The column identifier.</param>
    /// <param name="value">The new value.</param>
    /// <param name="previousValue">The old value.</param>
    public UpdateDataParametersSerializer(TItem item, string columnKey, object value, object previousValue)
    {
        Data = item;
        ColumnKey = columnKey;
        Value = value;
        PreviousValue = previousValue;
    }

    /// <summary>
    /// New instance for Add an item.
    /// </summary>
    /// <param name="item">The item.</param>

    public UpdateDataParametersSerializer(TItem item)
    {
        Data = item;
    }

    #endregion

}
