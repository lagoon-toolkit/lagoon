namespace Lagoon.UI.Components;

/// <summary>
/// Describe the event raised when user type text into search zone in LgSelect
/// </summary>
public class FilterChangeEventArgs : EventArgs
{

    /// <summary>
    /// The cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }
    
    /// <summary>
    /// The loading progression.
    /// </summary>
    public Progress Progress { get; }

    /// <summary>
    /// The text to use as filter.
    /// </summary>
    public string SearchText { get; }

    /// <summary>
    /// Alias to SearchText property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Value => SearchText;

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="searchText">The text to use as filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="progress">The loading progression.</param>
    public FilterChangeEventArgs(string searchText, CancellationToken cancellationToken, Progress progress)
    {
        SearchText = searchText;
        CancellationToken = cancellationToken;
        Progress = progress;
    }

}
