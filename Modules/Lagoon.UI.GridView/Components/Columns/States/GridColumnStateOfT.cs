namespace Lagoon.UI.Components.Internal;

internal class GridColumnState<TColumnValue> : GridColumnState
{

    /// <summary>
    /// Return the method to use when the FilterShowAllItems parameter is true.
    /// </summary>
    /// <returns>The method to use when the FilterShowAllItems parameter is true.</returns>
    internal virtual Func<CancellationToken, Task<IEnumerable<TColumnValue>>> GetFilterAllItems()
    {
        return null;
    }

}
