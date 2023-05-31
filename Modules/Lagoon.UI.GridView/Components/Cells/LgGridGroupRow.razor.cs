namespace Lagoon.UI.Components;

/// <summary>
/// Group cell
/// </summary>
public partial class LgGridGroupRow<TItem> : LgComponentBase
{
    #region fields

    /// <summary>
    /// List of group label
    /// </summary>
    private string _groupNames;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets level of the group row
    /// </summary>
    [Parameter]
    public GridGroupLevel<TItem> GroupData { get; set; }

    /// <summary>
    /// Gets or sets if group is collapsed or not
    /// </summary>
    [Parameter]
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Event rise on change collapse state of the group
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnChangeCollapse { get; set; }

    /// <summary>
    /// Gets or sets group items number
    /// </summary>
    [Parameter]
    public int Count { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets group parameters
    /// </summary>
    public GridGroupData<TItem> GroupContext { get; set; }

    #endregion

    #region Render fragments

    /// <summary>
    /// Gets render for group name
    /// </summary>
    private RenderFragment GroupNameRender =>
        builder =>
             {
                 int index = 0;
                 foreach (GridGroupingColumn group in GroupData.Params.Values)
                 {
                     builder.OpenElement(index++, "span");
                     builder.AddContent(index++, $"{group.Title} : {group.Value}");
                     builder.CloseElement();
                 }
             };

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets gridview parent
    /// </summary>
    [CascadingParameter]
    public LgGridView<TItem> GridView { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        GroupContext = new GridGroupData<TItem>(GroupData.Params, GroupData.Items.FirstOrDefault(), GroupNameRender);
        _groupNames = string.Join(", ", GroupData.Params.Values.Select(g => $"{g.Title} : {g.Value}"));
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-row");
        builder.Add("gridview-body");
        builder.Add("gridview-group-header");
        if (IsCollapsed)
        {
            builder.Add("collapse-group");
        }
        //if (GridView.HasFrozen)
        //{
        //    builder.Add("gridview-group-frozen");
        //}
    }

    /// <summary>
    /// Change collapse group state
    /// </summary>
    private void ChangeCollapse()
    {
        IsCollapsed = !IsCollapsed;
        if (OnChangeCollapse.HasDelegate)
        {
            OnChangeCollapse.TryInvokeAsync(App, IsCollapsed);
        }
    }

    #endregion
}
