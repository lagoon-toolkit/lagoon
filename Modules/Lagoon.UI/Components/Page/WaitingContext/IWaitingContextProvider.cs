namespace Lagoon.UI.Components;

internal interface IWaitingContextProvider
{

    /// <summary>
    /// Return a new waiting context with a cancellation token source.
    /// </summary>
    /// <returns>A new waiting context with a cancellation token source.</returns>
    WaitingContext GetNewWaitingContext();

}
