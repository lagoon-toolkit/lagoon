namespace Lagoon.Helpers;

/// <summary>
/// Provide an interface to export a list to a XLSX file.
/// </summary>
public class XslxExportProvider : IExportProvider
{

    #region properties

    ///<inheritdoc/>
    public virtual string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    ///<inheritdoc/>
    public virtual string FileExtension => ".xlsx";

    ///<inheritdoc/>
    public virtual string Id => "xlsx";

    ///<inheritdoc/>
    public virtual string DisplayName => "#ExportDisplayNameXLSX";

    ///<inheritdoc/>
    public virtual string IconName => "i-a-ExportExcel";

    /// <summary>
    /// Gets the export options.
    /// </summary>
    public XlsxOptions Options { get; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new export provider.
    /// </summary>
    public XslxExportProvider()
    {
        Options = new();
    }

    /// <summary>
    /// Initialise a new export provider.
    /// </summary>
    /// <param name="options">Export options.</param>
    public XslxExportProvider(XlsxOptions options)
    {
        Options = options;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public virtual byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns, CancellationToken cancellationToken)
    {
        //TODO Use cancellation parameter
        return XlsxDocument<T>.SerializeToByteArray(items, exportColumns, Options);
    }

    #endregion
}
