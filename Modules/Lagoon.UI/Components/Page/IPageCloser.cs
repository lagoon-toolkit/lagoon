namespace Lagoon.UI.Components.Internal;

internal interface IPageCloser
{

    /// <summary>
    /// Method called when the "Close" method of the page is called.
    /// </summary>
    /// <param name="page">Page to close.</param>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    Task ClosePageAsync(LgPage page, bool force);

}
