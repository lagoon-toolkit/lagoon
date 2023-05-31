namespace Lagoon.UI.Components;

/// <summary>
/// Information about the page activation.
/// </summary>
public class PageActivatedEventArgs : ActionEventArgs
{

    #region constructors

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    public PageActivatedEventArgs(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    #endregion

}
