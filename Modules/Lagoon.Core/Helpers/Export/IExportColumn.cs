namespace Lagoon.Helpers;

/// <summary>
/// Exportable column definition.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IExportColumn<TItem>
{
    /// <summary>
    /// Column title.
    /// </summary>
    public string ColumnTitle { get; }

    /// <summary>
    /// Group title
    /// </summary>
    public string ColumnGroupTitle { get; }

    /// <summary>
    /// Get the type of the value in that column.
    /// </summary>
    Type ValueType { get; }

    /// <summary>
    /// Get the value of an item in the column.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public object GetValue(TItem item);

    /// <summary>
    /// Set the value of an item in the column.
    /// </summary>
    /// <param name="item">Item to update.</param>
    /// <param name="value">The new value.</param>
    void SetValue(TItem item, object value);

}
