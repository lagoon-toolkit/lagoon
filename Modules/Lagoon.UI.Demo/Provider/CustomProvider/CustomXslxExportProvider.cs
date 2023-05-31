using Lagoon.UI.Demo.Provider.CustomXlsxDocument;

namespace Lagoon.UI.Demo.Provider.CustomProvider;

/// <summary>
/// Custom XslxExportProvider for example
/// </summary>
public class CustomXslxExportProvider : XslxExportProvider
{
    ///<inheritdoc/>
    public override byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns, CancellationToken cancellationToken)
    {
        return CustomXlsxDocument<T>.SerializeToByteArray(items, exportColumns, Options);
    }
}
