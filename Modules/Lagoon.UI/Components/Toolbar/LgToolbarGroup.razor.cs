namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar items group.
/// </summary>
public partial class LgToolbarGroup : LgComponentBase
{

    #region cascading parameters

    /// <summary>
    /// Parent toolbar object
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    private LgToolbar Toolbar { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the group kind
    /// </summary>
    [Parameter]
    public ToolbarGroupKind? Kind { get; set; }

    /// <summary>
    /// Gets or sets the child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Get the default kind from the toolbar
        if (!Kind.HasValue)
        {
            Kind = Toolbar.GroupKind;
        }
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        string _cssGrpKind = Kind switch
        {
            ToolbarGroupKind.Primary => "tbl-grp-primary",
            ToolbarGroupKind.Secondary => "tbl-grp-secondary",
            ToolbarGroupKind.Error => "tbl-grp-danger",
            ToolbarGroupKind.Info => "tbl-grp-info",
            ToolbarGroupKind.Success => "tbl-grp-success",
            ToolbarGroupKind.Warning => "tbl-grp-warning",
            ToolbarGroupKind.Ghost => "tbl-grp-ghost",
            _ => "",
        };
        builder.Add("btn-group toolbar-group", _cssGrpKind, CssClass);
    }

    #endregion

}
