namespace Lagoon.UI.Components;

/// <summary>
/// Treeview component
/// </summary>
public partial class LgTreeView<TItem> : LgComponentBase
{

    #region fields

    /// <summary>
    /// Reference to main html container tag
    /// </summary>
    private ElementReference _elementReference;

    /// <summary>
    /// The item equality comparer.
    /// </summary>
    private readonly IEqualityComparer<TreeNode<TItem>> _nodeEqualityComparer;

    /// <summary>
    /// The current selected node.
    /// </summary>
    private LgTreeViewNode<TItem> _selectedNode;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets items
    /// </summary>
    [Parameter]
    public TreeNodeCollection<TItem> Nodes { get; set; }

    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets display style
    /// </summary>
    [Parameter]
    public TreeviewKind Kind { get; set; } = TreeviewKind.None;

    /// <summary>
    /// Gets or sets the node click event
    /// </summary>
    [Parameter]
    public EventCallback<TreeNode<TItem>> OnNodeClick { get; set; }

    /// <summary>
    /// Gets or sets the selected node.
    /// </summary>
    [Parameter]
    public TreeNode<TItem> SelectedItem { get; set; }

    /// <summary>
    /// Event called when the selected node changed.
    /// </summary>
    [Parameter]
    public EventCallback<TreeNode<TItem>> SelectedItemChanged { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets displayed level content
    /// </summary>
    [Parameter]
    public RenderFragment<TreeNode<TItem>> NodeContent { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgTreeView()
    {
        _nodeEqualityComparer = EqualityComparer<TreeNode<TItem>>.Default;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("treeview", CssClass);
        switch (Kind)
        {
            case TreeviewKind.Card:
                builder.Add("trv-nestable");
                break;
            default:
                builder.Add("trv-default");
                break;
        }
    }

    /// <summary>
    /// Expand all the nodes of the tree.
    /// </summary>
    private void CollapseAll()
    {
        ChangeExpandedState(false);
    }

    /// <summary>
    /// Expand all the nodes of the tree.
    /// </summary>
    private void ExpandAll()
    {
        ChangeExpandedState(true);
    }

    /// <summary>
    /// Change the expanded state for all the nodes of the tree.
    /// </summary>
    /// <param name="expanded">The new expanded state.</param>
    private void ChangeExpandedState(bool expanded)
    {
        if (Nodes is not null)
        {
            foreach (TreeNode<TItem> item in Nodes)
            {
                item.ChangeAllExpandedState(expanded);
            }
        }
    }

    /// <summary>
    /// Move keys management
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnKeyUpAsync(KeyboardEventArgs args)
    {
        if (args.Key is "ArrowUp" or "ArrowDown")
        {
            await JS.InvokeVoidAsync("Lagoon.JsUtils.moveFocus", _elementReference, args.Key == "ArrowUp" ? MoveFocusEnum.Up : MoveFocusEnum.Down);
        }
        if (args.Key == "Home")
        {
            await JS.InvokeVoidAsync("Lagoon.JsUtils.moveFocus", _elementReference, MoveFocusEnum.First);
        }
        if (args.Key == "End")
        {
            await JS.InvokeVoidAsync("Lagoon.JsUtils.moveFocus", _elementReference, MoveFocusEnum.Last);
        }
    }

    /// <summary>
    /// Indicate if the node must be selected.
    /// </summary>
    /// <param name="state">The node to select.</param>
    /// <returns><c>true</c> if the node is the selected one.</returns>
    internal bool IsSelectedNode(TreeNode<TItem> state)
    {
        return _nodeEqualityComparer.Equals(SelectedItem, state);
    }

    /// <summary>
    /// Method called to select a treeview node.
    /// </summary>
    /// <param name="node"></param>
    internal async Task<bool> SelectNodeAsync(LgTreeViewNode<TItem> node)
    {
        if (ReferenceEquals(_selectedNode, node))
        {
            return false;
        }
        LgTreeViewNode<TItem> previous = _selectedNode;
        _selectedNode = node;
        if (previous is not null)
        {
            try
            {
                previous.Unselect();
            }
            catch (ObjectDisposedException)
            {
                // We ignore the unselect of the previous node if it's no more exists
            }
        }
        if (SelectedItemChanged.HasDelegate)
        {
            await SelectedItemChanged.TryInvokeAsync(App, _selectedNode.State);
        }
        else
        {
            SelectedItem = _selectedNode.State;
        }
        return true;
    }

    #endregion

}

/// <summary>
/// Enum used with moveFocus JS call
/// </summary>
public enum MoveFocusEnum
{
    /// <summary>
    /// Focus to first focusable element
    /// </summary>
    First = -1,
    /// <summary>
    /// Focus to previous focusable element
    /// </summary>
    Up = 0,
    /// <summary>
    /// Focus to next focusable element inside the parent
    /// </summary>
    Right = 1,
    /// <summary>
    /// Focus to next focusable element
    /// </summary>
    Down = 2,
    /// <summary>
    /// Focus to previous focusable element inside the parent
    /// </summary>
    Left = 3,
    /// <summary>
    /// Focus to last focusable element
    /// </summary>
    Last = 4,
}

/// <summary>
/// Treeview display style
/// </summary>
public enum TreeviewKind
{
    /// <summary>
    /// Initial style 
    /// </summary>
    None,
    /// <summary>
    /// Card style
    /// </summary>
    Card
}
