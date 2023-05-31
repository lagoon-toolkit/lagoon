namespace Lagoon.UI.Components;

/// <summary>
/// Toolbar button
/// </summary>
public partial class LgToolbarButton : LgButton
{

    #region cascading parameters

    /// <summary>
    /// Parent toolbar object
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    private LgToolbar Toolbar { get; set; }

    /// <summary>
    /// Parent ToolbarMenu object
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    protected LgToolbarMenu ToolbarMenuParent { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize new instance.
    /// </summary>
    public LgToolbarButton()
    {
        Kind = (ButtonKind)(-1);
        TooltipPosition = TooltipPosition.Bottom;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Check button size customization from the toolbar
        ButtonSize = Toolbar?.ButtonSize ?? ButtonSize.Medium;
        // If the kind parameter isn't specified, we use the toolbar kind
        if ((int)Kind == -1)
        {
            Kind = Toolbar?.ButtonKind ?? ButtonKind.Secondary;
        }
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        if (ToolbarMenuParent is null)
        {
            base.OnBuildClassAttribute(builder);
        }
        else
        {
            builder.Add("dropdown-item d-flex");
            builder.AddIf(Disabled, "disabled");
            builder.Add(CssClass);
        }
    }

    #endregion

}
