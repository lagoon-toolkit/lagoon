using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lagoon.Model.Collation;

internal static class MySQLExtension
{
    /// <summary>
    /// Add Lagoon PostgreSQL extension
    /// </summary>
    /// <param name="optionsBuilder"></param>    
    /// <returns></returns>
    public static DbContextOptionsBuilder ConfigureMySQLCollation(this DbContextOptionsBuilder optionsBuilder)
    {
        MySQLDbContextOptionsExtension extension = optionsBuilder.Options
                                 .FindExtension<MySQLDbContextOptionsExtension>()
                   ?? new MySQLDbContextOptionsExtension();
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        return optionsBuilder;
    }

}

/// <summary>
/// Lagoon PostgreSQL extensions
/// </summary>
public class MySQLDbContextOptionsExtension : IDbContextOptionsExtension
{

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    /// <summary>
    /// Constructor
    /// </summary>
    public MySQLDbContextOptionsExtension()
    {
    }

    ///<inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    /// <summary>
    /// Add service plugin
    /// </summary>
    /// <param name="services"></param>
    void IDbContextOptionsExtension.ApplyServices(IServiceCollection services)
    {
        services.AddScoped<IMethodCallTranslatorPlugin, MySQLCallTranslatorPlugin>(s =>
        {
            return new MySQLCallTranslatorPlugin(s.GetService<ISqlExpressionFactory>());
        });
    }

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "MySQLExtensionInfo";

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
}

/// <summary>
/// MySQL SQL translator plugin
/// </summary>
public class MySQLCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>    
    public MySQLCallTranslatorPlugin(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    /// Gets translators
    /// </summary>
    public IEnumerable<IMethodCallTranslator> Translators => new IMethodCallTranslator[] {
        new MySQLCallTranslator(_sqlExpressionFactory)
    };
}

/// <summary>
/// Case and accent insensitive translator
/// </summary>
public class MySQLCallTranslator : IMethodCallTranslator
{
    #region fields

    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    /// String contains method definition
    /// </summary>
    private static readonly MethodInfo _contains = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string), typeof(StringComparison) });

    /// <summary>
    /// String startWith method definition
    /// </summary>
    private static readonly MethodInfo _endWith = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) });

    /// <summary>
    /// String endWith method definition
    /// </summary>
    private static readonly MethodInfo _startWith = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) });

    #endregion

    #region methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>   
    public MySQLCallTranslator(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    /// SQL method translation
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public SqlExpression Translate(SqlExpression instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method == _contains)
        {
            // Replace Locate by Like to add search collation support
            return TranslateContains(instance, arguments[0]);
        }
        return null;
    }

    /// <summary>
    /// SQL expression for contains method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    private SqlExpression TranslateContains(SqlExpression instance, SqlExpression pattern)
    {
        Microsoft.EntityFrameworkCore.Storage.RelationalTypeMapping stringTypeMapping = Microsoft.EntityFrameworkCore.Query.ExpressionExtensions.InferTypeMapping(instance!, pattern);
        instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
        pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);
        LikeExpression strinCheck = _sqlExpressionFactory.Like(
                _sqlExpressionFactory.ApplyTypeMapping(instance!, stringTypeMapping),
                _sqlExpressionFactory.Add(
                    _sqlExpressionFactory.Add(
                        _sqlExpressionFactory.Constant("%"),
                        pattern
                    ),
                    _sqlExpressionFactory.Constant("%")
                    )
                );
        return pattern is SqlConstantExpression constantPattern
            ? (string)constantPattern.Value == string.Empty
                ? _sqlExpressionFactory.Constant(true)
                : strinCheck
            : (SqlExpression)_sqlExpressionFactory.OrElse(
            _sqlExpressionFactory.Equal(
                pattern,
                _sqlExpressionFactory.Constant(string.Empty, stringTypeMapping)),
            strinCheck);
    }

    #endregion


}
