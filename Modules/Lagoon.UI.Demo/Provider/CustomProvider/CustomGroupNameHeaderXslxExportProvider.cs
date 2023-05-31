using Lagoon.UI.Demo.Provider.CustomXlsxDocument;

namespace Lagoon.UI.Demo.Provider.CustomProvider;

/// <summary>
/// Custom XslxExportProvider for example
/// </summary>
public class CustomGroupNameHeaderXslxExportProvider : XslxExportProvider
{
    ///<inheritdoc/>
    public override byte[] SerializeToByteArray<T>(IEnumerable<T> items, ExportColumnCollection<T> exportColumns, CancellationToken cancellationToken)
    {
        Options.GroupNameOnDistinctRow = true;
        return CustomXlsxDocument<T>.SerializeToByteArray(items, exportColumns, Options);
    }
}
