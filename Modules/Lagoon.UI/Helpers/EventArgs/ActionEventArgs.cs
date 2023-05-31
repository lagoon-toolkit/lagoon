namespace Lagoon.UI.Components;

/// <summary>
/// Event with "CancelationToken".
/// </summary>
public class ActionEventArgs
{

    #region fields

    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    private readonly CancellationToken? _cancellationToken;

    #endregion

    #region properties

    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    public CancellationToken CancellationToken
    {
        get
        {
            if (_cancellationToken.HasValue)
                return _cancellationToken.Value;
            throw new Exception("The CancellationToken isn't available in this context.");
        }
    }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    public ActionEventArgs()
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="waitingContext">Waiting context.</param>
    public ActionEventArgs(WaitingContext waitingContext)
        : this(waitingContext?.CancellationToken)
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    public ActionEventArgs(CancellationToken? cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    #endregion

}
