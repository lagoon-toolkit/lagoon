using Lagoon.Core.Application.Logging;
using Lagoon.Server.Exceptions;

namespace Lagoon.Server.Application.Logging;

/// <summary>
/// A simplified console logger.
/// </summary>
internal class LgConsoleLogger : LgLogger
{

    #region static fields

    /// <summary>
    /// The sync lock identifier.
    /// </summary>
    private static object _lock = new();

    #endregion

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    /// <summary>
    /// Minimum level to log.
    /// </summary>
    private LogLevel _minimumLevel;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application</param>
    /// <param name="name">The category name.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <param name="alwaysTrace">Indicate if all messages are traced.</param>
    public LgConsoleLogger(ILgApplication app, string name, bool isAppCategory, bool alwaysTrace)
        : base(name, isAppCategory)
    {
        _app = app;
        _minimumLevel = alwaysTrace || isAppCategory ? LogLevel.Trace : LogLevel.Warning;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public override bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minimumLevel;
    }

    ///<inheritdoc/>
    protected override void Log(LogLevel logLevel, DateTime time, string message, string stackTrace, string category, bool isAppCategory, LogSide side, Exception exception)
    {
        string context = null;
        // Get the HTTP request informations
        HttpRequest request = exception is ContextProxyException ex
            ? ex.Context?.Request
            : _app.HttpContextAccessor?.HttpContext?.Request;           
        if (request is not null)
        {
            context = $"{request.Method} {request.Path}{request.QueryString}";
        }
        // Log the message
        Log(logLevel, message, stackTrace, category, side, context);
    }

    /// <summary>
    /// Write the event to the console.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="stackTrace">The stack trace associated to the message.</param>
    /// <param name="category">The category</param>
    /// <param name="side">The application source of the event.</param>
    /// <param name="context">The HTTP context informations.</param>
    public static void Log(LogLevel level, string message, string stackTrace = null, string category = null,
        LogSide side = LogSide.Server, string context = null)
    {
        // Log to the console
        lock (_lock)
        {
            string code = null;
            switch (level)
            {
                case LogLevel.Trace:
                    code = "trce";
                    break;
                case LogLevel.Debug:
                    code = "dbug";
                    break;
                case LogLevel.Information:
                    code = "info";
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case LogLevel.Warning:
                    code = "warn";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    code = "fail";
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case LogLevel.Critical:
                    code = "crit";
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            switch (level)
            {
                case LogLevel.Error:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.Critical:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
            }
            Console.Write(code);
            Console.ResetColor();
            Console.Write(": ");
            if (side != LogSide.Server)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Write(side.ToString().ToLowerInvariant());
                Console.ResetColor();
                Console.Write(": ");
            }
            if (category is not null)
            {
                Console.WriteLine(category);
                Console.Write("      ");
            }
            Console.WriteLine(message);
            if (context is not null)
            {
                Console.Write("      ");
                Console.WriteLine(context);
            }
            if (stackTrace is not null)
            {
                Console.Write("      ");
                // We keep only the first line + the lines with line number.
                bool firstLine = true;
                using (System.IO.StringReader sr = new(stackTrace))
                {
                    string line;
                    while ((line = sr.ReadLine()) is not null)
                    {
                        if (firstLine)
                        {
                            Console.WriteLine(line);
                            firstLine = false;
                        }
                        else if ((line.Contains(":line ") || line.StartsWith(" --->"))
                                && !line.Contains(nameof(Middlewares.LagoonResourceMiddleware)) // The [StackTraceHidden] on the invoke method doesn't seams to work
                                )
                        {
                            Console.WriteLine(line);
                        }

                    }
                }
            }
        }
    }

    #endregion

}
