namespace Lagoon.Core.Application.Logging;


/// <summary>
/// The Lagoon file logger.
/// </summary>
public class LgFileLoggerProvider : LgLoggerProvider<LgFileLogger>
{

    #region properties

    /// <summary>
    /// The shared message handler instance.
    /// </summary>
    public LgFileLoggerManager Manager { get; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize a new file logger.
    /// </summary>
    /// <param name="app">The Main class.</param>
    /// <param name="options">Logger options.</param>
    public LgFileLoggerProvider(ILgApplicationBase app, LgFileLoggerOptions options)
        : base(app)
    {
        Manager = new LgFileLoggerManager(options);
    }

    #endregion

    #region dispose

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Manager.Dispose();
        }
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override LgFileLogger CreateNewLogger(string categoryName, bool isAppCategory)
    {
        return new LgFileLogger(Manager, categoryName, isAppCategory);
    }

    #endregion

}
