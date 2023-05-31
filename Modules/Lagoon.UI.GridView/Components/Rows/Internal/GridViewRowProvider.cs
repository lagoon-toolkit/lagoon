namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Row provider
/// </summary>
/// <typeparam name="TItem"></typeparam>
internal class GridViewRowProvider<TItem>
{
    /// <summary>
    /// Row enumerator
    /// </summary>
    private readonly IEnumerator<TItem> _enumerator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data"></param>
    public GridViewRowProvider(IEnumerable<TItem> data)
    {
        _enumerator = data.GetEnumerator();
    }

    /// <summary>
    /// Return enumerator
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryGetRow(out TItem item)
    {
        if (_enumerator.MoveNext())
        {
            item = _enumerator.Current;
            return true;
        }
        item = default;
        return false;
    }
}
