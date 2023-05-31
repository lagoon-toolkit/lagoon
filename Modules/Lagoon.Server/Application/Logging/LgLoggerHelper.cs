namespace Lagoon.Server.Application.Logging;

internal static class LgLoggerHelper
{

    /// <summary>
    /// Indicate this category trace message is useless, we only keep the message of the exception object.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns><c>It's a logger from and unhandled error handler.</c></returns>
    public static bool IsUnhandleExceptionCategory(string category)
    {
        return category is "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"
            or "Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware";
    }

}
