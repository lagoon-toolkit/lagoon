namespace Lagoon.UI.Components;

/// <summary>
/// Show centred message with icon, title and description.
/// </summary>
public partial class LgBigMessage : LgComponentBase
{

    #region parameters

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [Parameter]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// The content of the message.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("big-msg", CssClass);
    }

    #endregion region

}
