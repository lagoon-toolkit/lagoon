namespace Lagoon.UI.Components;

/// <summary>
/// A Global Search item.
/// </summary>
public class LgGlobalSearchItem : LgPageLink
{
    #region properties

    /// <summary>
    /// Gets or sets the Text.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Text { get => Title; }

    #endregion

    #region constructors

    /// <summary>
    /// New link instance.
    /// </summary>
    /// <param name="uri">URI of the page.</param>
    /// <param name="title">Title for the page.</param>
    public LgGlobalSearchItem(string uri, string title):base(uri,title) { }

    #endregion

}
