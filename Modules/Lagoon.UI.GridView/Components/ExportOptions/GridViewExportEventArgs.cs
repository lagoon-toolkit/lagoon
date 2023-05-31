namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Contains 'export options' event data.
/// </summary>
public class GridViewExportEventArgs : EventArgs
{

    /// <summary>
    /// The export options.
    /// </summary>
    public ExportOptions Options { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="options">The exprt oprions.</param>
    public GridViewExportEventArgs(ExportOptions options)
    {
        Options = options;
    }

}
