namespace Lagoon.UI.Components;

/// <summary>
/// Event argument for the items loading.
/// </summary>
public class GetItemsFiltredByTextAsyncEventArgs<TItemValue> : EventArgs
{
    #region fields

    private GetItemsAsyncEventArgs<TItemValue> _e;

    #endregion

    #region properties

    /// <summary>
    /// Filter items containing the the search text.
    /// </summary>
    /// <remarks>Remark : It can't be null ! When they're is no filter, the value is string.empty.</remarks>
    public string SearchedText => _e.SearchedText;

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken CancellationToken => _e.CancellationToken;

    /// <summary>
    /// The progression manager.
    /// </summary>
    public Progress Progress => _e.Progress;

    #endregion

    #region constructors

    /// <summary>
    /// Initilialise a new instance.
    /// </summary>
    /// <param name="e">Arguments.</param>
    public GetItemsFiltredByTextAsyncEventArgs(GetItemsAsyncEventArgs<TItemValue> e)
    {
        _e = e;
    }

    #endregion

}
