namespace Lagoon.UI.Components;

/// <summary>
/// Class to encapsulate a block of code with encapsulate a long treatment.
/// </summary>
public class PageWaitingContext : WaitingContext
{

    #region fields

    /// <summary>
    /// Page associated.
    /// </summary>
    private readonly LgPage _page;

    #endregion

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    /// <param name="page">Page.</param>
    public PageWaitingContext(LgPage page)
    {
        _page = page;
        _page._waitingContextList.Add(this);
    }

    #region disposing

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _page._waitingContextList.Remove(this);
        _page.StopWaiting();
        base.Dispose(disposing);
    }

    #endregion

}
