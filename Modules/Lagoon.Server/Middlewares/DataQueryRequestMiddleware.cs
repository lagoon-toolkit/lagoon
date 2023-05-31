using Lagoon.Shared.Model;

namespace Lagoon.Server.Middlewares;

/// <summary>
/// Data query middleware
/// </summary>
public class DataQueryRequestMiddleware
{

    #region fields

    /// <summary>
    /// Next delegate
    /// </summary>
    private RequestDelegate _next;

    /// <summary>
    /// Data query identifier
    /// </summary>
    private string _uriPrefix = IDataQueryRequest.QueryIdentifier;

    #endregion

    #region constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next"></param>
    public DataQueryRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    #endregion

    #region methods

    /// <summary>
    /// Change uri and data request for data query
    /// </summary>        
    /// <param name="context">The HTTP context.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        // Detect if the request is data query
        HttpRequest request = context.Request;
        if (request.Path.Value.EndsWith($"/{_uriPrefix}"))
        {
            // Extract api route and routing to GET
            string requesPath = request.Path.Value;
            string newPath = requesPath[..requesPath.LastIndexOf($"/{_uriPrefix}", StringComparison.OrdinalIgnoreCase)];
            request.Path = newPath;
            request.Method = HttpMethods.Get;
            request.Headers.Add(IDataQueryRequest.HeaderQueryIdentifier, bool.TrueString);
            request.EnableBuffering();
        }
        return _next(context);
    }



    #endregion

}
