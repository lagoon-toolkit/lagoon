namespace Lagoon.Helpers;

/// <summary>
/// Provide an interface to export a list to a file.
/// </summary>
public interface IExportProvider
{

    #region properties

    /// <summary>
    /// Get the MIME type of the export.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Get the file extension of the export.
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Get the identifier of the export.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Get the display name of the export.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Get the icon name of the export.
    /// </summary>
    string IconName { get; }

    #endregion

    #region methods

    /// <summary>
    /// Serialize items and get content into a byte array.
    /// </summary>
    /// <typeparam name="T">Type of an item.</typeparam>
    /// <param name="items">List of items.</param>
    /// <param name="exportColumns">List of columns informations to export.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns></returns>
    byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns, CancellationToken cancellationToken);

    /// <summary>
    /// Serialize items and get content into a byte array.
    /// </summary>
    /// <typeparam name="T">Type of an item.</typeparam>
    /// <param name="items">List of items.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns></returns>
    byte[] SerializeToByteArray<T>(IEnumerable<T> items, CancellationToken cancellationToken)
    {
        return SerializeToByteArray<T>(items, null, new CancellationToken());
    }

    /// <summary>
    /// Serialize items and get content into a byte array.
    /// </summary>
    /// <typeparam name="T">Type of an item.</typeparam>
    /// <param name="items">List of items.</param>
    /// <param name="exportColumns">List of columns informations to export.</param>
    /// <returns></returns>
    byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns)
    {
        return SerializeToByteArray<T>(items,exportColumns,new CancellationToken());
    }

    /// <summary>
    /// Serialize items and get content into a byte array.
    /// </summary>
    /// <typeparam name="T">Type of an item.</typeparam>
    /// <param name="items">List of items.</param>
    /// <returns></returns>
    byte[] SerializeToByteArray<T>(IEnumerable<T> items)
    {
        return SerializeToByteArray<T>(items, null, new CancellationToken());
    }

    #endregion

}
