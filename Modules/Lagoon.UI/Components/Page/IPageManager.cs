namespace Lagoon.UI.Components.Internal;

internal interface IPageManager
{

    /// <summary>
    /// Event called when the tab is selected.
    /// </summary>
    public event Func<Task> OnRaiseActivatedEventAsync;

    /// <summary>
    /// Event called when the tab is closing.
    /// </summary>
    public event Func<PageClosingSourceEvent, Func<Task>, Task<bool>> OnRaiseClosingEventAsync;

    /// <summary>
    /// Event called when the tab is closed.
    /// </summary>
    public event Func<Task> OnRaiseCloseEventAsync;

}
