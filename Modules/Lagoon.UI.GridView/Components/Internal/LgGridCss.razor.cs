namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Gridview CSS builder
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgGridCss<TItem>
{
    #region properties

    /// <summary>
    /// Gets or sets gridview Id
    /// </summary>
    [Parameter]
    public LgGridView<TItem> GridView { get; set; }

    #endregion

    /// <summary>
    /// Force render
    /// </summary>
    public void Refresh()
    {
        StateHasChanged();
    }
}
