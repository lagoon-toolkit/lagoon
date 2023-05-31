namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Cell containing the filter for the column.
/// </summary>
public abstract partial class LgGridFilterCellBase : ComponentBase
{

    #region fields

    private FilterContext _filterContext;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets column of the cell
    /// </summary>

    [CascadingParameter]
    public LgGridBaseColumn Column { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The content of the cell.
    /// </summary>
    protected RenderFragment<FilterContext> RenderFilterContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _filterContext = new(Column.State);
    }

    /// <summary>
    /// Force cell rendering
    /// </summary>
    public void Update()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Return the content to set in "class" attribute.
    /// </summary>
    /// <returns>The content to set in "class" attribute.</returns>
    private string GetCssClassAttribute()
    {
        LgCssClassBuilder builder = new(Column.State.GetCellCssClass());
        OnBuildCssClassAttribute(builder);
        return builder.ToString();
    }

    /// <summary>
    /// Build the content of the CSS class.
    /// </summary>
    /// <param name="builder">The builder.</param>
    protected virtual void OnBuildCssClassAttribute(LgCssClassBuilder builder)
    {
        if (Column.State.Frozen)
        {
            builder.Add("gridview-cell-frozen");
        }
    }

    #endregion

}
