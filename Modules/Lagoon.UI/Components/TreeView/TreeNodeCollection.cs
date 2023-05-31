using System.Collections;

namespace Lagoon.UI.Components;

/// <summary>
/// Tree node collection.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TreeNodeCollection<TItem> : ICollection<TreeNode<TItem>>
{

    #region fields

    private List<TreeNode<TItem>> _nodes = new();

    #endregion

    #region properties

    /// <summary>
    /// Gets the node at the specific index.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public TreeNode<TItem> this[int i] => _nodes[i];

    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// </summary>
    public int Count => _nodes.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    bool ICollection<TreeNode<TItem>>.IsReadOnly => false;

    /// <summary>
    /// The node containing the collection.
    /// </summary>
    public TreeNode<TItem> Node { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public TreeNodeCollection()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="node">The node containing the collection.</param>
    public TreeNodeCollection(TreeNode<TItem> node)
    {
        Node = node;
    }

    #endregion

    #region methods

    /// <summary>
    /// Add a new node.
    /// </summary>
    /// <param name="item">The item to associate with the node.</param>
    /// <returns>The new node.</returns>
    public TreeNode<TItem> Add(TItem item)
    {
        TreeNode<TItem> node = new(item);
        Add(node);
        node.SetSiblings(this);
        return node;
    }

    /// <summary>
    /// Add a new node.
    /// </summary>
    public void Add(TreeNode<TItem> node)
    {
        _nodes.Add(node);
        node.SetSiblings(this);
    }

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public void Clear()
    {
        _nodes.Clear();
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="node">The object to locate in the collection.</param>
    /// <returns>true if item is found in the collection otherwise, false.</returns>
    public bool Contains(TreeNode<TItem> node)
    {
        return _nodes.Contains(node);
    }

    /// <summary>
    /// Copy the collection to an array.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(TreeNode<TItem>[] array, int arrayIndex)
    {
        _nodes.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the collection.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public bool Remove(TreeNode<TItem> node)
    {
        return _nodes.Remove(node);
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An enumerator that iterates through a collection.</returns>
    public IEnumerator<TreeNode<TItem>> GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An enumerator that iterates through a collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    /// <summary>
    /// Do an action for each node in this collection.
    /// </summary>
    /// <param name="action">Action to do.</param>
    public void ForEach(Action<TreeNode<TItem>> action)
    {
        foreach (TreeNode<TItem> node in _nodes)
        {
            action(node);
        }
    }

    /// <summary>
    /// Do an action on all the node of the tree.
    /// </summary>
    /// <param name="action">Action to do.</param>
    public void ForEachInHierarchy(Action<TreeNode<TItem>> action)
    {
        foreach (TreeNode<TItem> node in EnumerateHierarchy())
        {
            action(node);
        }
    }

    /// <summary>
    /// List all the nodes of the tree.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TreeNode<TItem>> EnumerateSelected()
    {
        return EnumerateHierarchy().Where(n => n.IsChecked);
    }

    /// <summary>
    /// List all the nodes in the hierarchy.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TreeNode<TItem>> EnumerateHierarchy()
    {
        Stack<TreeNode<TItem>> stack = new();
        for (int i = _nodes.Count - 1; i >= 0; i--)
        {
            stack.Push(_nodes[i]);
        }
        while (stack.Count > 0)
        {
            TreeNode<TItem> node = stack.Pop();
            yield return node;
            TreeNodeCollection<TItem> children = node.Children;
            if (children is not null)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    stack.Push(children[i]);
                }
            }
        }
    }

    #endregion
}
