using Lagoon.Core.Application.Logging;

namespace Lagoon.UI.Application.Logging;

internal class LgClientLoggerProvider : LgLoggerProvider<LgClientLogger>
{

    #region fields

    /// <summary>
    /// The shared message handler instance.
    /// </summary>
    private readonly LgClientLoggerManager _manager;

    #endregion

    #region constructors

    /// <summary>
    /// Initialise the logger provider.
    /// </summary>
    /// <param name="app">The current applisation.</param>
    public LgClientLoggerProvider(LgApplication app) : base(app)
    {
        _manager = new LgClientLoggerManager(app);
    }

    #endregion

    #region dispose

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _manager.Dispose();
        }
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override LgClientLogger CreateNewLogger(string categoryName, bool isAppCategory)
    {
        return new LgClientLogger(_manager, categoryName, isAppCategory);
    }

    #endregion

}

