namespace Lagoon.UI.Components;

/// <summary>
/// Information about the page loading.
/// </summary>
public class PageLoadEventArgs : ActionEventArgs
{

    #region constructors

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    public PageLoadEventArgs(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    #endregion

}
