namespace Lagoon.Server.Exceptions;

internal class ContextProxyException : Exception
{

    /// <summary>
    /// The original exception.
    /// </summary>
    private readonly Exception _innerException;

    /// <summary>
    /// The HTTP context.
    /// </summary>
    public HttpContext Context { get; }

    /// <summary>
    /// The stack trace of the original error.
    /// </summary>
    public override string StackTrace => _innerException.StackTrace;

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <param name="innerException">The origical exception.</param>
    public ContextProxyException(HttpContext context, Exception innerException)
        : base(innerException.Message)
    {
        _innerException = innerException;
        Context = context;
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return _innerException.ToString();
    }

}
