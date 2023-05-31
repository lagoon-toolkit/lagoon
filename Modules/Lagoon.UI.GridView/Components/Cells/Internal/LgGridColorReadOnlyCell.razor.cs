namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Readonly Color picker cell
/// </summary>
public partial class LgGridColorReadOnlyCell : ComponentBase
{
    #region properties

    /// <summary>
    /// Gets or sets the cell value
    /// </summary>
    [Parameter]
    public string CellValue { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return the content to set in "class" attribute.
    /// </summary>
    /// <returns></returns>
    protected static string GetClassAttribute()
    {
        LgCssClassBuilder builder = new();
        builder.Add("colorPickerBox-dropdown");
        return builder.ToString();
    }

    #endregion
}
