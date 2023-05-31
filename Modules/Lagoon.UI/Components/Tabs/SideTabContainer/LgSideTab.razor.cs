namespace Lagoon.UI.Components;

/// <summary>
/// Tab of the side container
/// </summary>
public partial class LgSideTab : LgCustomTab
{

    #region render fragments

    /// <summary>
    /// Gets or sets the tab button content.
    /// </summary>
    [Parameter]
    public RenderFragment ButtonContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.AddIf(ButtonContent is null, "nav-link lg-side-tab");
        base.OnBuildClassAttribute(builder);
    }

    /// <summary>
    /// Return the ARIA and tooltip attributes for the tab button.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return Tab.GetButtonAdditionalAttributes(!App.BehaviorConfiguration.RgaaSupport, TabContainer.TooltipPosition);
    }

    #endregion

}
