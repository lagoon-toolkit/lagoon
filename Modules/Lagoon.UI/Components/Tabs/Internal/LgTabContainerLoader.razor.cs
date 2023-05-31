namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component to load the tab list.
/// </summary>
public partial class LgTabContainerLoader : LgComponentBase
{

    #region fields

    private Guid _initTabDataUpdateState;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets tab container
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgCustomTabContainer TabContainer { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets content of the tab container
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        _initTabDataUpdateState = TabContainer.TabDataUpdateState;
        return base.ShouldRender();
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        TabContainer.TabDataList = TabContainer.LoadingTabDataList;
        // if they're new tab data, ask the render of the button and the content part
        if (_initTabDataUpdateState != TabContainer.TabDataUpdateState)
        {
            TabContainer.OnRefreshHeader?.Invoke();
            TabContainer.OnRefreshBody?.Invoke();
        }
    }

    #endregion

}
