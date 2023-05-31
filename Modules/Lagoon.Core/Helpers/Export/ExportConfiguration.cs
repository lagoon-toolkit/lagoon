namespace Lagoon.UI.Components;

/// <summary>
/// Configuration export options
/// </summary>
public class ExportConfiguration
{
    /// <summary>
    /// Default export format for enum column
    /// </summary>
    public ExportEnumFormat ExportEnumFormat { get; set; } = ExportEnumFormat.DisplayName;

    /// <summary>
    /// Default export format for select column
    /// </summary>
    public ExportFormat ExportFormat { get; set; } = ExportFormat.Text;

    /// <summary>
    /// The language to use if the <see cref="ExportEnumFormat"/> property is <see cref="ExportEnumFormat.DisplayName"/>.
    /// If ValueFormatLanguage is <c>null</c> or undefined, the current language is used.
    /// </summary>
    public string ExportFormatLanguage { get; set; }
}
