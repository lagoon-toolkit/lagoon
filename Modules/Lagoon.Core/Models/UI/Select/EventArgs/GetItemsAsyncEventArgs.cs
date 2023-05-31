namespace Lagoon.UI.Components;

/// <summary>
/// Event argument for the items loading.
/// </summary>
public class GetItemsAsyncEventArgs<TItemValue> : EventArgs
{

    #region properties

    /// <summary>
    /// Values with unknown extra informations (Title, IconName, ...)
    /// </summary>
    public List<TItemValue> UnknownValues { get; }

    /// <summary>
    /// Only selected values are needed.
    /// </summary>
    public bool OnlyUnknown => UnknownValues?.Count > 0;

    /// <summary>
    /// Filter items containing the the search text.
    /// </summary>
    /// <remarks>Remark : It can't be null.</remarks>
    public string SearchedText { get; }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// The progression manager.
    /// </summary>
    public Progress Progress { get; }

    #endregion

    #region constructors

    /// <summary>
    /// Initilialise a new instance.
    /// </summary>
    /// <param name="unknownValues">If not null, only unknown values are needed, else the searchText value must be used.</param>
    /// <param name="searchText">Filter items containing the the search text.</param>
    /// <param name="progress">The progression manager.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public GetItemsAsyncEventArgs(List<TItemValue> unknownValues, string searchText, Progress progress, CancellationToken cancellationToken)
    {
        UnknownValues = unknownValues;
        SearchedText = searchText;
        CancellationToken = cancellationToken;
        Progress = progress;
    }

    #endregion

}
