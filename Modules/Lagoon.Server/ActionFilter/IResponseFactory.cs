using Lagoon.Shared.Model;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Queryable response builder
/// </summary>
public interface IResponseFactory
{
    /// <summary>
    /// Return data filtered, sorted and paged
    /// </summary>
    /// <param name="query">The query.</param>        
    /// <param name="request">The query request.</param>        
    /// <returns></returns>    
    object GetResponse(IQueryable query, IDataQueryRequest request);
}