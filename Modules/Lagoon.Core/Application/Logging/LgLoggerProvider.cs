using System.Collections.Concurrent;

namespace Lagoon.Core.Application.Logging;

/// <summary>
/// Abstract class to handle categories.
/// </summary>
/// <typeparam name="TLogger">The type of logger.</typeparam>
public abstract class LgLoggerProvider<TLogger> : ILoggerProvider
    where TLogger : ILogger
{

    #region fields

    /// <summary>
    /// The namespace of the application.
    /// </summary>
    private string _appNamespace;

    /// <summary>
    /// The default category used by the application main trace method.
    /// </summary>
    private string _defaultCategory;

    /// <summary>
    /// Indicate if the object has alredy been released.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// The dictionary of logger by category.
    /// </summary>
    private readonly ConcurrentDictionary<string, TLogger> _loggers;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The Main class.</param>
    public LgLoggerProvider(ILgApplicationBase app)
    {
        _loggers = new();
        _defaultCategory = app.GetType().FullName;
        _appNamespace = _defaultCategory.Split('.')[0] + ".";
    }

    #endregion

    #region dispose

    /// <summary>
    /// Free resources.
    /// </summary>
    /// <param name="disposing">Free managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _loggers.Clear();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Free resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region methods

    /// <summary>
    /// Create a console logger that show only warning and errors except for the current application messages.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <returns>a new logger.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        string key;
        // We don't keep the category name for "{ProjectNameSpace}.Main"
        if (string.IsNullOrEmpty(categoryName) || IsNullCategory(categoryName))
        {
            categoryName = null;
            key = string.Empty;
        }
        else
        {
            key = categoryName;
        }
        // We try to return the existing logger, else create a new one
        return _loggers.TryGetValue(key, out TLogger logger)
            ? logger
            : _loggers.GetOrAdd(key, CreateNewLogger(categoryName, categoryName is null || categoryName.StartsWith(_appNamespace)));
    }

    /// <summary>
    /// Create a new logger.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <returns>The new logger.</returns>
    protected abstract TLogger CreateNewLogger(string categoryName, bool isAppCategory);

    /// <summary>
    /// Indicate if the category must be replaced by null.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns><c>true</c> if the category must be replaced by null.</returns>
    protected virtual bool IsNullCategory(string category)
    {
        return category == _defaultCategory;
    }

    #endregion

}
