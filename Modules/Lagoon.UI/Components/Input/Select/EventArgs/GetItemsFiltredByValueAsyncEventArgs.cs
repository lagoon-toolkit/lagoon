namespace Lagoon.UI.Components;

/// <summary>
/// Event argument for the items loading.
/// </summary>
public class GetItemsFiltredByValueAsyncEventArgs<TItemValue> : EventArgs
{
    #region fields

    private GetItemsAsyncEventArgs<TItemValue> _e;

    #endregion

    #region properties

    /// <summary>
    /// Selected values with unknown extra informations (Title, IconName, ...)
    /// </summary>
    /// <remarks>Remark: It's cannot be null or empty !</remarks>
    public List<TItemValue> SearchedValues => _e.UnknownValues;

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
    public GetItemsFiltredByValueAsyncEventArgs(GetItemsAsyncEventArgs<TItemValue> e)
    {
        _e = e;
    }

    #endregion

}
