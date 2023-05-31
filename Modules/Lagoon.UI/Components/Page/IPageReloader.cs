namespace Lagoon.UI.Components.Internal;

internal interface IPageReloader
{

    /// <summary>
    /// Method called when the method "Reload" is called in a LgPage.
    /// </summary>
    /// <param name="page">Page.</param>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    Task ReloadPageAsync(LgPage page, bool force);

}
