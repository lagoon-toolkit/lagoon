namespace Lagoon.UI.Components;

/// <summary>
/// Arguments of the active tab event
/// </summary>
public class ActivateTabEventArgs : EventArgs
{

    #region properties

    /// <summary>
    /// Gets unique id of the active tab
    /// </summary>
    public ITab Tab { get; }

    #endregion

    #region construtor

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="tab">Tab identifier.</param>
    public ActivateTabEventArgs(ITab tab)
    {
        Tab = tab;
    }

    #endregion

}
