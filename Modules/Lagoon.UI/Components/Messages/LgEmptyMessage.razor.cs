namespace Lagoon.UI.Components;

/// <summary>
/// Empty message component.
/// </summary>
public partial class LgEmptyMessage : LgComponentBase
{

    #region fields

    private string _description;

    private string _title;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the Class css
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the icon
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    [Parameter]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _title = Title.CheckTranslate();
        _description = Description.CheckTranslate();
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("empty-msg text-center", CssClass);
    }

    #endregion region

}
