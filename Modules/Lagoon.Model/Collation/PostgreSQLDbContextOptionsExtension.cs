using Lagoon.Helpers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Lagoon.Model.Collation;

/// <summary>
/// Lagoon PostgreSQL extensions
/// </summary>
public class PostgreSQLDbContextOptionsExtension : IDbContextOptionsExtension
{
    #region private class

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "PostgreSQLExtensionInfo";

        public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        { }

        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return string.Equals(LogFragment, other.LogFragment, StringComparison.Ordinal);
        }
    }

    #endregion

    #region fields

    /// <summary>
    /// Selected collation type
    /// </summary>
    private readonly CollationType _collationType;

    #endregion

    #region properties

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    DbContextOptionsExtensionInfo IDbContextOptionsExtension.Info => new ExtensionInfo(this);

    #endregion

    #region constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PostgreSQLDbContextOptionsExtension(CollationType collationType)
    {
        _collationType = collationType;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    void IDbContextOptionsExtension.Validate(IDbContextOptions options)
    { }

    /// <summary>
    /// Add service plugin
    /// </summary>
    /// <param name="services"></param>
    void IDbContextOptionsExtension.ApplyServices(IServiceCollection services)
    {
        services.AddScoped<IMethodCallTranslatorPlugin, PostgreSQLCallTranslatorPlugin>(s =>
        {
            return new PostgreSQLCallTranslatorPlugin(
                s.GetService<ISqlExpressionFactory>(),
                s.GetService<IModel>(),
                s.GetService<ITypeMappingSource>(),
                _collationType);
        });
    }

    #endregion

}