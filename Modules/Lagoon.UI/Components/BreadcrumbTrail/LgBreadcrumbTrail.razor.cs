namespace Lagoon.UI.Components;

/// <summary>
/// Breadcrumb component
/// </summary>
public partial class LgBreadcrumbTrail : LgAriaComponentBase
{
    #region render fragments

    /// <summary>
    /// Content of the breadcrumb.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Sublist of MenuCustomItem.
    /// </summary>
    [Parameter]
    public BreadcrumbTrailItemList Items { get; set; }

    /// <summary>
    /// Sublist of MenuCustomItem.
    /// </summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Parameter]
    public BreadcrumbTrailItemList Data { get => Items; set => Items = value; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
    }

    #endregion region

}
