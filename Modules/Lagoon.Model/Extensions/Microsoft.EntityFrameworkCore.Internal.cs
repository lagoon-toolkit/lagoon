using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.ComponentModel;

namespace Lagoon.Server.Extensions;

/// <summary>
/// Extension to unsafe EntityFrameworkCore methods.
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// In the case of an EF Core query, checks if the query can be translated into SQL.
    /// Always returns <c>true</c> for other types of Query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns><c>true</c> if it's valid query.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsValidQuery(this IQueryable query)
    {
        if(query.Provider is EntityQueryProvider)
        {
            try
            {
                _ = query.ToQueryString();
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Try to find the DbContext associated to the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The DbContext associated to the query.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DbContext TryGetDbContext(this IQueryable query)
    {
        //see : https://stackoverflow.com/questions/53198376/net-ef-core-2-1-get-dbcontext-from-iqueryable-argument
        if (query.Provider is not EntityQueryProvider)
        {
            return null;
        }
#pragma warning disable EF1001 // Internal EF Core API usage.
        BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        QueryCompiler queryCompiler = (QueryCompiler)typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
        RelationalQueryContextFactory queryContextFactory = (RelationalQueryContextFactory)typeof(QueryCompiler).GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);
        QueryContextDependencies dependencies = (QueryContextDependencies)typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags).GetValue(queryContextFactory);
        return dependencies.StateManager.Context;
#pragma warning restore EF1001 // Internal EF Core API usage.
    }

}
