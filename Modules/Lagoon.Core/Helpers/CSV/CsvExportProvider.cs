namespace Lagoon.Helpers;

/// <summary>
/// Provide an interface to export a list to a CSV file.
/// </summary>
public class CsvExportProvider : IExportProvider
{

    #region properties

    ///<inheritdoc/>
    public virtual string ContentType => "text/csv";

    ///<inheritdoc/>
    public virtual string FileExtension => ".csv";

    ///<inheritdoc/>
    public virtual string Id => "csv";

    ///<inheritdoc/>
    public virtual string DisplayName => "#ExportDisplayNameCSV";

    ///<inheritdoc/>
    public virtual string IconName => "i-a-ExportCsv";

    /// <summary>
    /// Gets the export options.
    /// </summary>
    public CsvOptions Options { get; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new export provider.
    /// </summary>
    public CsvExportProvider()
    {
        Options = new();
    }

    /// <summary>
    /// Initialise a new export provider.
    /// </summary>
    /// <param name="options">Export options.</param>
    public CsvExportProvider(CsvOptions options)
    {
        Options = options;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public virtual byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns, CancellationToken cancellationToken)
    {
        //TODO Use cancellation parameter
        return CsvDocument<T>.SerializeToByteArray(items, exportColumns, Options);
    }

    #endregion
}
