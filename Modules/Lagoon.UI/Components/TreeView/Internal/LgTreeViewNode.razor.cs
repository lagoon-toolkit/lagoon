namespace Lagoon.UI.Components.Internal;

/// <summary>
/// The node of a treeview.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgTreeViewNode<TItem> : ComponentBase, IDisposable
{

    #region fields

    /// <summary>
    /// Indicate if the current node has been disposed.
    /// </summary>
    private bool _disposed;

    #endregion

    #region properties

    /// <summary>
    /// Indicate if the node is selected.
    /// </summary>
    public bool IsSelected => TreeView.IsSelectedNode(State);

    #endregion

    #region cascading parameters

    /// <summary>
    /// The parent treeview.
    /// </summary>
    [CascadingParameter]
    public LgTreeView<TItem> TreeView { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the state of the node.
    /// </summary>
    [Parameter]
    public TreeNode<TItem> State { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Free the node resources.
    /// </summary>
    /// <param name="disposing">Indicate the managed resources should be free.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }

    /// <summary>
    /// Free the object from the memory.
    /// </summary>
    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Keyboard key actions in the treeview
    /// </summary>
    /// <param name="args"></param>
    private void OnKeyDown(KeyboardEventArgs args)
    {
        // Expand or close treenode
        if (args.Key == "ArrowRight" || args.Key == "ArrowLeft")
        {
            State.ChangeExpandedState(args.Key == "ArrowRight");
        }
    }

    /// <summary>
    /// Node click event management
    /// </summary>
    private async Task NodeClickAsync()
    {
        // Select current node
        bool stateHasChanged = await TreeView.SelectNodeAsync(this);
        // Raise the clic event
        if (TreeView.OnNodeClick.HasDelegate)
        {
            await TreeView.OnNodeClick.TryInvokeAsync(TreeView.App, State);
        }
        else if (stateHasChanged)
        {
            StateHasChanged();
        }
    }

    /// <summary>
    /// The node class.
    /// </summary>
    /// <param name="isOpen">Open state.</param>
    /// <returns>The CSS class.</returns>
    private static string NodeClass(bool isOpen)
    {
        LgCssClassBuilder builder = new();
        builder.AddIf(isOpen, "treeview-open");
        return builder.ToString();
    }

    /// <summary>
    /// The node content CSS class.
    /// </summary>
    /// <returns>The CSS class.</returns>
    private string NodeContentClass()
    {
        LgCssClassBuilder builder = new("trv-node-content");
        builder.AddIf(IsSelected, "trv-node-selected");
        return builder.ToString();
    }

    /// <summary>
    /// Method called when a node loose the selection.
    /// </summary>
    internal void Unselect()
    {
        if (!_disposed)
        {
            StateHasChanged();
        }
    }

    #endregion

}
