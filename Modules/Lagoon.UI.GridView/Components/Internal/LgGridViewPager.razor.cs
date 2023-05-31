namespace Lagoon.UI.GridView.Components.Internal;

/// <summary>
/// GridView pager
/// </summary>
public partial class LgGridViewPager : LgPager
{

    #region fields

    /// <summary>
    /// Indicate than component has values
    /// </summary>
    private bool _isInitialized;

    private int? SetCurrentPage { get; set; } = null;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the input page selector is shown
    /// </summary>
    [Parameter]
    public bool DisplayInputPager { get; set; }

    /// <summary>
    /// Gets or sets current page size
    /// </summary>
    [Parameter]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the list of page size
    /// </summary>
    [Parameter]
    public int[] PaginationSizeSelector { get; set; }
    

    /// <summary>
    /// Gets or sets page change event
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnPageSizeChangeAsync { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _isInitialized = true;
    }

    /// <summary>
    /// Allows the user to select a particular page.
    /// </summary>
    /// <param name="args"></param>
    private async void ChangeCurrentPageAsync(KeyboardEventArgs args)
    {
        if((args.Code == "Enter" || args.Code == "NumpadEnter") && SetCurrentPage.HasValue)
        {
            if (OnChange.HasDelegate)
            {
                await OnChange.TryInvokeAsync(App, new ChangeEventArgs() { Value = (int)SetCurrentPage });
            }
        }
    }

    #endregion

}
