namespace Lagoon.UI.Components;

/// <summary>
/// Gridview loader indicator
/// </summary>
public class LgGridLoaderView<TItem> : LgLoaderView
{
    #region fields

    /// <summary>
    /// The Id of the loading request.
    /// </summary>
    private Guid _loadDataState = Guid.Empty;

    /// <summary>
    /// Indicate if we must reload the data.
    /// </summary>
    private bool _loadData;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets content to display after loading
    /// </summary>
    [Parameter]
    public Guid LoadDataState { get; set; }

    /// <summary>
    /// Gets or sets loading indicator
    /// </summary>
    [CascadingParameter]
    public LgGridView<TItem> GridView { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if(LoadDataState != _loadDataState)
        {
            _loadData = true;
            _loadDataState = LoadDataState;
        }
        IsLoading = _loadData;
    }

    #endregion
}
