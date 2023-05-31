using System.Collections;

namespace Lagoon.UI.Components;

/// <summary>
/// Item for LgTreeView
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TreeNode<TItem> : IEnumerable<TreeNode<TItem>>
{

    #region fields

    private TreeNodeCollection<TItem> _children;
    private TreeNodeCollection<TItem> _siblings;

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets if the node is checked.
    /// </summary>
    public bool? Checked { get; set; } = false;

    /// <summary>
    /// Gets children list.
    /// </summary>
    public TreeNodeCollection<TItem> Children
    {
        get
        {
            _children ??= new(this);
            return _children;
        }
    }

    /// <summary>
    /// Gets or sets the expanded state.
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// Gets if the node has children.
    /// </summary>
    public bool HasChildren => _children is not null && _children.Count > 0;

    /// <summary>
    /// Gets or sets if the node is hidden in the tree view.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Gets if the node is selected.
    /// </summary>
    public bool IsChecked => Checked.HasValue && Checked.Value;

    /// <summary>
    /// Gets or sets the item for the node.
    /// </summary>
    public TItem Item { get; set; }

    /// <summary>
    /// Gets the parent node of the current node. <c>null</c> if it's a root node.
    /// </summary>
    public TreeNode<TItem> Parent => Siblings.Node;

    /// <summary>
    /// Siblings collection.
    /// </summary>
    public TreeNodeCollection<TItem> Siblings => _siblings;

    #endregion

    #region constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeItem">Node item</param>
    public TreeNode(TItem nodeItem)
    {
        Item = nodeItem;
    }

    #endregion

    #region methods

    /// <summary>
    /// Set th
    /// </summary>
    /// <param name="items"></param>
    internal void SetSiblings(TreeNodeCollection<TItem> items)
    {
        _siblings = items;
    }

    /// <summary>
    /// Add a new children.
    /// </summary>
    /// <param name="item">The item to associate with the node.</param>
    public TreeNode<TItem> AddChild(TItem item)
    {
        return Children.Add(item);
    }

    /// <summary>
    /// Toggle collapse state
    /// </summary>
    public void ToggleCollapseState()
    {
        Expanded = !Expanded;
    }

    /// <summary>
    /// Change the expanded state for current item and his children.
    /// </summary>
    /// <param name="expanded">The new expanded state.</param>
    public void ChangeAllExpandedState(bool expanded)
    {
        Expanded = expanded;
        if (_children != null)
        {
            foreach (TreeNode<TItem> node in Children)
            {
                node.ChangeAllExpandedState(expanded);
            }
        }
    }

    /// <summary>
    /// Change the expanded state for current item.
    /// </summary>
    /// <param name="expanded"></param>
    public void ChangeExpandedState(bool expanded)
    {
        Expanded = expanded;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that iterates through the collection.</returns>
    public IEnumerator<TreeNode<TItem>> GetEnumerator()
    {
        if (_children is null)
        {
            return Enumerable.Empty<TreeNode<TItem>>().GetEnumerator();
        }
        else
        {
            return _children.GetEnumerator();
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that iterates through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Check or uncheck a node in the tree.
    /// </summary>
    /// <param name="checked">The new state of the node.</param>
    public void CheckNode(bool @checked)
    {
        Checked = @checked;
        // Set all the sub node in indeterminate state if checked, else uncheck all children
        bool? childSelected = @checked ? null : false;
        Children?.ForEachInHierarchy(n => n.Checked = childSelected);
        // Update sibling and parent nodes
        TreeNode<TItem> child = this;
        TreeNode<TItem> parent = child.Parent;
        while (parent is not null)
        {
            if (parent.Checked is null || parent.Checked.Value)
            {
                // Check siblings
                foreach (TreeNode<TItem> sibling in child.Siblings)
                {
                    if (sibling != child)
                    {
                        sibling.Checked ??= true;
                    }
                }
            }
            // Change parent selection
            parent.Checked = HasCheckedChildren(parent) ? null : false;
            // Move to the next level
            child = parent;
            parent = child.Parent;
        }
    }

    /// <summary>
    /// Check if a node has a selected node in his children.
    /// </summary>
    /// <param name="node">The node/</param>
    /// <returns>Return true if the node has a selected child.</returns>
    private static bool HasCheckedChildren(TreeNode<TItem> node)
    {
        if (node.HasChildren)
        {
            foreach (TreeNode<TItem> child in node.Children)
            {
                if (child.Checked is null || child.Checked.Value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

}
