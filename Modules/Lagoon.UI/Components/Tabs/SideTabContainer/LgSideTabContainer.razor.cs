namespace Lagoon.UI.Components;

/// <summary>
/// Tab container with navigation in side panel.
/// </summary>
public partial class LgSideTabContainer : LgCustomTabContainer
{

    #region parameters

    /// <summary>
    /// Gets or sets css class of the header
    /// </summary>
    [Parameter]
    public string CssClassHead { get; set; } = "col-2";

    /// <summary>
    /// Gets or sets css class of the container
    /// </summary>
    [Parameter]
    public string CssClassContent { get; set; } = "col-10";

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    public LgSideTabContainer()
    {
        TooltipPosition = TooltipPosition.Top;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("row side-tab-container");
        builder.Add(CssClass);
    }

    /// <summary>
    /// Return the CSS classes of the left panel.
    /// </summary>
    /// <returns>The CSS classes of the left panel.</returns>
    protected virtual string GetSidePanelCssClass()
    {
        var builder = new LgCssClassBuilder();
        builder.Add("side-tab-nav", CssClassHead);
        return builder.ToString();
    }

    /// <summary>
    /// Return the CSS classes of the left panel.
    /// </summary>
    /// <returns>The CSS classes of the left panel.</returns>
    protected virtual string GetContentPanelCssClass()
    {
        var builder = new LgCssClassBuilder();
        builder.Add("side-tab-content", CssClassContent);
        return builder.ToString();
    }

    #endregion

}
