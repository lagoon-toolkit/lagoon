namespace Lagoon.UI.Components;

/// <summary>
/// Class to encapsulate a block of code with encapsulate a long treatment.
/// </summary>
public class WaitingContext : IDisposable
{

    #region fields

    /// <summary>
    /// Flag used to track if we already have disposed this component
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Cancellation token source.
    /// </summary>
    private readonly CancellationTokenSource _cts;

    #endregion

    #region properties

    /// <summary>
    /// Gets the System.Threading.CancellationToken associated.
    /// </summary>
    public CancellationToken CancellationToken => _cts.Token;

    #endregion

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    public WaitingContext()
    {
        _cts = new CancellationTokenSource();
    }

    #region disposing

    /// <summary>
    /// Don't modify the content of this method. If you want to free some resources, use <c>Dispose(bool disposing)</c>
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Free used resources
    /// </summary>
    /// <param name="disposing">Will be always true, we don't use finalize in this component</param>
    protected virtual void Dispose(bool disposing)
    {
        // If already disposed, we have nothing to do
        if (_isDisposed)
        {
            return;
        }
        // Disposing component resources
        if (disposing)
        {
            _cts.Dispose();
        }
        // Flag component as disposed
        _isDisposed = true;
    }

    #endregion

    #region methods

    /// <summary>
    /// Communicates a request for cancellation.
    /// </summary>
    internal void Cancel() {
        _cts.Cancel();
    }

    #endregion

}
