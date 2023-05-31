namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Column option class
/// </summary>
public class ExportOptions
{
    /// <summary>
    /// Gets or sets export only displayed columns
    /// </summary>
    public ExportColumnsMode ExportColumnsMode { get; set; }

    /// <summary>
    /// Gets or sets export with filters
    /// </summary>
    public ExportRowMode ExportRowMode { get; set; }

    /// <summary>
    /// Gets or sets export providers list
    /// </summary>
    public IExportProvider ExportProvider { get; set; }

    /// <summary>
    /// Gets or sets export providers list
    /// </summary>
    public string ExportProviderId { get; set; }

}
