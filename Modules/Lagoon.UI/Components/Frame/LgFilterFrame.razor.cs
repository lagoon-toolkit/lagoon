namespace Lagoon.UI.Components;

/// <summary>
/// Filter frame component.
/// </summary>
public partial class LgFilterFrame : LgFrame
{

    #region Public properties

    /// <summary>
    /// Gets or sets the frame child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildListViewContent { get; set; }

    /// <summary>
    /// Gets or sets the frame child content view more
    /// </summary>
    [Parameter]
    public RenderFragment ChildContentViewMore { get; set; }

    /// <summary>
    /// Gets or sets the reset filters toolbar button action
    /// </summary>
    [Parameter]
    public EventCallback<FilterFrameResetEventArgs> OnResetFilters { get; set; }

    /// <summary>
    /// Gets or sets the reset filters toolbar button action
    /// </summary>
    [Parameter]
    public EventCallback<FilterFrameViewMoreEventArgs> OnViewMoreFilters { get; set; }

    #endregion

    #region private properties

    /// <summary>
    /// View more state
    /// </summary>
    private bool _showViewMore = false;

    /// <summary>
    /// Toolbar label item 'view more'
    /// </summary>
    private string _lblBtnViewMore;

    /// <summary>
    /// Toolbar icon item 'view more'
    /// </summary>
    private string _iconBtnViewMore;

    #endregion

    #region methods

    /// <summary>
    /// Toolbar render
    /// </summary>
    /// <returns></returns>
    protected override RenderFragment RenderToolbarContent()
    {
        return (builder) =>
        {
            builder.AddContent(1, Toolbar);

            builder.OpenComponent<LgToolbarButton>(2);
            builder.AddAttribute(3, nameof(LgToolbarButton.IconName), IconNames.All.ArrowClockwise);
            builder.AddAttribute(4, nameof(LgToolbarButton.Text), "lblFrmFiltersReset".Translate());
            builder.AddAttribute(5, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ResetFiltersAsync));
            builder.CloseComponent();

            if (ChildContentViewMore != null)
            {
                GetViewMoreLabelIcon();
                builder.OpenComponent<LgToolbarButton>(6);
                builder.AddAttribute(7, nameof(LgToolbarButton.IconName), _iconBtnViewMore);
                builder.AddAttribute(8, nameof(LgToolbarButton.Text), _lblBtnViewMore);
                builder.AddAttribute(9, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ViewMoreFiltersAsync));
                builder.CloseComponent();
            }
        };
    }

    ///<inheritdoc/>
    protected override RenderFragment RenderBodyContent()
    {
        return (renderBodyContent) =>
        {
            int index = 0;
            renderBodyContent.OpenRegion(index++);
            renderBodyContent.AddContent(index++, ChildListViewContent);
            if (_showViewMore)
            {
                renderBodyContent.OpenElement(index++, "div");
                renderBodyContent.AddAttribute(index++, "class", "filtersViewMore");
                renderBodyContent.AddContent(index++, @ChildContentViewMore);
                renderBodyContent.CloseElement();
            }
            renderBodyContent.CloseRegion();
        };

    }

    /// <summary>
    /// View more button label and icon
    /// </summary>
    private void GetViewMoreLabelIcon()
    {
        _lblBtnViewMore = _showViewMore ? "lblFrmFiltersViewLess".Translate() : "lblFrmFiltersViewMore".Translate();
        _iconBtnViewMore = _showViewMore ? IconNames.All.Dash : IconNames.All.Plus;
    }

    #endregion

    #region events

    /// <summary>
    /// Reset filters action
    /// </summary>
    private async Task ResetFiltersAsync()
    {
        if (OnResetFilters.HasDelegate)
        {
            await OnResetFilters.TryInvokeAsync(App, new FilterFrameResetEventArgs());
        }
    }

    /// <summary>
    /// View more action (toolbar)
    /// </summary>
    private async Task ViewMoreFiltersAsync()
    {
        _showViewMore = !_showViewMore;
        // Complete view more event 
        if (OnViewMoreFilters.HasDelegate)
        {
            await OnViewMoreFilters.TryInvokeAsync(App, new FilterFrameViewMoreEventArgs()
            {
                ShowViewMore = _showViewMore
            });
        }
        StateHasChanged();
    }

    #endregion

}
