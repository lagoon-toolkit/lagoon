namespace Lagoon.UI.Components;

/// <summary>
/// Finder component.
/// </summary>
public partial class LgFinder : LgAriaComponentBase
{

    #region public properties

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Result content
    /// </summary>
    [Parameter]
    public RenderFragment ResultContent { get; set; }

    /// <summary>
    /// Filter content
    /// </summary>
    [Parameter]
    public RenderFragment FilterContent { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the button label when the results are shown
    /// </summary>
    [Parameter]
    public string ButtonResultLabel { get; set; } = "#finderViewFilters";

    /// <summary>
    /// Gets or sets the button label when the filters are shown
    /// </summary>
    [Parameter]
    public string ButtonFilterLabel { get; set; } = "#finderViewResults";

    /// <summary>
    /// Gets or sets the title css
    /// </summary>
    [Parameter]
    public string TitleCssClass { get; set; }

    /// <summary>
    /// Gets or sets a callback when switch from filters to results.
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnFilter { get; set; }

    #endregion

    #region private properties

    /// <summary>
    /// Button Label
    /// </summary>
    private string buttonLabel = "";

    /// <summary>
    /// Weither the filters content is shown or not
    /// </summary>
    private bool showFilters = false;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        buttonLabel = showFilters ? ButtonFilterLabel : ButtonResultLabel;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("finder", CssClass);
    }

    #endregion

    #region event

    /// <summary>
    /// Content toggle between results and filters
    /// </summary>
    private async Task ToggleContentAsync()
    {
        if (showFilters)
        {
            await FilterAsync();
        }
        showFilters = !showFilters;
        buttonLabel = showFilters ? ButtonFilterLabel : ButtonResultLabel;
    }

    private Task FilterAsync()
    {
        return ExecuteActionAsync(OnFilter);
    }

    #endregion
}
