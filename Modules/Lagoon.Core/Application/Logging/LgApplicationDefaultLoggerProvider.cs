namespace Lagoon.Core.Application.Logging;


/// <summary>
/// Custom file logger
/// </summary>
internal class LgApplicationDefaultLoggerProvider : ILoggerProvider
{

    #region Private properties

    /// <summary>
    /// Logger provider instance.
    /// </summary>
    private readonly ILoggerProvider _provider;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize a new file logger.
    /// </summary>
    /// <param name="provider">The application default logger provider.</param>
    public LgApplicationDefaultLoggerProvider(ILoggerProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Return the application default logger.
    /// </summary>
    /// <param name="categoryName">CategoryName</param>
    /// <returns>The application default logger.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return _provider.CreateLogger(categoryName);
    }

    /// <summary>
    /// Freeing resources.
    /// </summary>
    /// <param name="disposing">Free the managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        // Don't dispose the logger, it's done by the application
    }

    /// <summary>
    /// Freeing resources.
    /// </summary>
    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }

#endregion

}
