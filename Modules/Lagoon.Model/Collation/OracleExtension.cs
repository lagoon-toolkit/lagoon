using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace Lagoon.Model.Collation;

internal static class OracleExtension
{
    /// <summary>
    /// Add Lagoon PostgreSQL extension
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <param name="collationName"></param>
    /// <returns></returns>
    public static DbContextOptionsBuilder ConfigureOracleCollation(this DbContextOptionsBuilder optionsBuilder, string collationName)
    {
        OracleDbContextOptionsExtension extension = optionsBuilder.Options
                                 .FindExtension<OracleDbContextOptionsExtension>()
                   ?? new OracleDbContextOptionsExtension(collationName);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        return optionsBuilder;
    }

}

/// <summary>
/// Lagoon PostgreSQL extensions
/// </summary>
public class OracleDbContextOptionsExtension : IDbContextOptionsExtension
{
    /// <summary>
    /// Selected collation name
    /// </summary>
    private readonly string _collationName;

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    /// <summary>
    /// Constructor
    /// </summary>
    public OracleDbContextOptionsExtension(string collationName)
    {
        _collationName = collationName;
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
        services.AddScoped<IMethodCallTranslatorPlugin, OracleCallTranslatorPlugin>(s =>
        {
            return new OracleCallTranslatorPlugin(
                s.GetService<ISqlExpressionFactory>(),
                _collationName);
        });
    }

    /// <summary>
    /// PostgreSQLExtensionInfo Extension info
    /// </summary>
    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "OracleExtensionInfo";

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
/// Oracle SQL translator plugin
/// </summary>
public class OracleCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    /// Collation name
    /// </summary>
    private readonly string _collationName;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>
    /// <param name="collationName"></param>        
    public OracleCallTranslatorPlugin(ISqlExpressionFactory sqlExpressionFactory, string collationName)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _collationName = collationName;
    }

    /// <summary>
    /// Gets translators
    /// </summary>
    public IEnumerable<IMethodCallTranslator> Translators => new IMethodCallTranslator[] {
        new OracleCallTranslator(_sqlExpressionFactory, _collationName)
    };
}

/// <summary>
/// Case and accent insensitive translator
/// </summary>
public class OracleCallTranslator : IMethodCallTranslator
{
    #region fields

    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    /// Current collation name
    /// </summary>
    private readonly string _collationName;

    /// <summary>
    /// String contains method definition
    /// </summary>
    private static readonly MethodInfo _contains = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) })!;

    /// <summary>
    /// String startWith method definition
    /// </summary>
    private static readonly MethodInfo _endWith = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) })!;

    /// <summary>
    /// String endWith method definition
    /// </summary>
    private static readonly MethodInfo _startWith = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) })!;

    #endregion

    #region methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>        
    /// <param name="collationName"></param>
    public OracleCallTranslator(ISqlExpressionFactory sqlExpressionFactory, string collationName)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _collationName = collationName;
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
            return TranslateContains(instance, arguments[0]);
        }
        bool startWith = method == _startWith;
        return startWith || method == _endWith ? TranslateStartsEndsWith(instance, arguments[0], startWith) : null;
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
        SqlBinaryExpression strinCheck = _sqlExpressionFactory.GreaterThan(
            _sqlExpressionFactory.Function(
                "instr",
                new[]
                {
                    new CollateExpression(_sqlExpressionFactory.ApplyTypeMapping(instance!, stringTypeMapping), _collationName),
                    _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping)
                },
                nullable: true,
                argumentsPropagateNullability: new bool[] { true, true },
                typeof(int)),
            _sqlExpressionFactory.Constant(0));

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

    /// <summary>
    /// SQL expression for start or end with methods
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="pattern"></param>
    /// <param name="startsWith"></param>
    /// <returns></returns>
    private SqlExpression TranslateStartsEndsWith(SqlExpression instance, SqlExpression pattern, bool startsWith)
    {
        return _sqlExpressionFactory.Like(
                    new CollateExpression(instance, _collationName),
                    startsWith ? _sqlExpressionFactory.Add(pattern, _sqlExpressionFactory.Constant("%"))
                    : _sqlExpressionFactory.Add(_sqlExpressionFactory.Constant("%"), pattern));
    }

    #endregion


}



/// <summary>
/// Merge two string Sql expression with space between
/// </summary>
internal class CombineSqlExpression : SqlExpression
{
    /// <summary>
    ///     The left operand.
    /// </summary>
    public virtual SqlExpression Left { get; }

    /// <summary>
    ///     The right operand.
    /// </summary>
    public virtual SqlExpression Right { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public CombineSqlExpression(
        SqlExpression left,
        SqlExpression right) : base(left.Type, left.TypeMapping)
    {
        Left = left;
        Right = right;
    }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        SqlExpression left = (SqlExpression)visitor.Visit(Left);
        SqlExpression right = (SqlExpression)visitor.Visit(Right);

        return Update(left, right);
    }

    /// <summary>
    ///     Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will
    ///     return this expression.
    /// </summary>
    /// <param name="left">The <see cref="Left" /> property of the result.</param>
    /// <param name="right">The <see cref="Right" /> property of the result.</param>
    /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
    public CombineSqlExpression Update(SqlExpression left, SqlExpression right)
    {
        return left != Left || right != Right
            ? new CombineSqlExpression(left, right)
            : this;
    }

    ///<inheritdoc/>
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        bool requiresBrackets = RequiresBrackets(Left);

        if (requiresBrackets)
        {
            expressionPrinter.Append("(");
        }
        expressionPrinter.Visit(Left);
        if (requiresBrackets)
        {
            expressionPrinter.Append(")");
        }
        requiresBrackets = RequiresBrackets(Right);
        if (requiresBrackets)
        {
            expressionPrinter.Append("(");
        }
        expressionPrinter.Visit(Right);
        if (requiresBrackets)
        {
            expressionPrinter.Append(")");
        }
        static bool RequiresBrackets(SqlExpression expression)
            => expression is SqlBinaryExpression or LikeExpression;
    }
}
