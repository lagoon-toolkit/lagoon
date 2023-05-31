using Lagoon.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using System.Text;

namespace Lagoon.Model.Collation;

/// <summary>
/// Case and accent insensitive translator
/// </summary>
public class PostgreSQLCallTranslator : IMethodCallTranslator
{

    #region fields

    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    /// Current collation type
    /// </summary>
    private readonly CollationType _collationType;

    /// <summary>
    /// DB string mapping
    /// </summary>
    private readonly CoreTypeMapping _textTypeMapping;

    /// <summary>
    /// Escape character
    /// </summary>
    private const char LikeEscapeChar = '\\';

    /// <summary>
    /// String contains method definition
    /// </summary>
    private static readonly MethodInfo _contains = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] { typeof(string) });

    /// <summary>
    /// String startWith method definition
    /// </summary>
    private static readonly MethodInfo _endWith = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });

    /// <summary>
    /// String endWith method definition
    /// </summary>
    private static readonly MethodInfo _startWith = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

    /// <summary>
    /// Accent sensitive and case insensitive indicator
    /// </summary>
    private bool _asCi;

    /// <summary>
    /// Accent sensitive and case insensitive indicator
    /// </summary>
    private readonly bool _aiCi;

    #endregion

    #region methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>
    /// <param name="model"></param>
    /// <param name="typeMappingSource"></param>
    /// <param name="collationType"></param>
    public PostgreSQLCallTranslator(ISqlExpressionFactory sqlExpressionFactory, IModel model, ITypeMappingSource typeMappingSource, CollationType collationType)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _collationType = collationType;
        _textTypeMapping = typeMappingSource.FindMapping(typeof(string), model);
        _aiCi = _collationType == CollationType.IgnoreCaseAndAccent;
        _asCi = _collationType == CollationType.IgnoreCase;
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
    /// SQL Lower function
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    private SqlFunctionExpression Lower(SqlExpression argument)
    {
        return _sqlExpressionFactory.Function("lower",
            new[] { argument },
            nullable: true,
            argumentsPropagateNullability: new bool[] { true, true },
            typeof(string));
    }

    /// <summary>
    /// SQL Unaccent function
    /// </summary>
    /// <param name="argument"></param>    
    /// <returns></returns>
    private SqlFunctionExpression UnAccent(SqlExpression argument)
    {
        return _sqlExpressionFactory.Function("unaccent",
            new[] { argument },
            nullable: true,
            argumentsPropagateNullability: new bool[] { true, true },
            typeof(string));
    }

    /// <summary>
    /// Return expression transformed to sensitive support
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    private SqlFunctionExpression SensitiveTransform(SqlExpression argument)
    {
        return _aiCi ? UnAccent(Lower(argument)) : _asCi ? Lower(argument) : null;
    }

    /// <summary>
    /// SQL expression for contains method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    private SqlExpression TranslateContains(SqlExpression instance, SqlExpression pattern)
    {
        RelationalTypeMapping stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance!, pattern);
        instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
        pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);

        SqlBinaryExpression strposCheck = _sqlExpressionFactory.GreaterThan(
            _sqlExpressionFactory.Function(
                "strpos",
                new[]
                {
                    SensitiveTransform(instance!),
                    SensitiveTransform(pattern),
                },
                nullable: true,
                argumentsPropagateNullability: new bool[] { true, true },
                typeof(int)),
            _sqlExpressionFactory.Constant(0));

        return pattern is SqlConstantExpression constantPattern
            ? (string)constantPattern.Value == string.Empty
                ? _sqlExpressionFactory.Constant(true)
                : strposCheck
            : (SqlExpression)_sqlExpressionFactory.OrElse(
            _sqlExpressionFactory.Equal(
                pattern,
                _sqlExpressionFactory.Constant(string.Empty, stringTypeMapping)),
            strposCheck);
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
        RelationalTypeMapping stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, pattern);

        instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
        pattern = _sqlExpressionFactory.ApplyTypeMapping(pattern, stringTypeMapping);

        if (pattern is SqlConstantExpression constantExpression)
        {
            SqlExpression sqlSearchTerm = constantExpression.Value is string constantPattern
                ? SensitiveTransform(_sqlExpressionFactory.Constant(
                        startsWith
                            ? EscapeLikePattern(constantPattern) + '%'
                            : '%' + EscapeLikePattern(constantPattern)))
                : (SqlExpression)SensitiveTransform(_sqlExpressionFactory.Constant(null, stringTypeMapping));
            return _sqlExpressionFactory.Like(SensitiveTransform(instance), sqlSearchTerm);
        }
        // The pattern is non - constant, we use LEFT or RIGHT to extract substring and compare.
        // For StartsWith we also first run a LIKE to quickly filter out most non-matching results(sargable, but imprecise
        // because of wildchars).
        SqlExpression uaInstanceExpression = SensitiveTransform(instance);
        SqlExpression leftRight = _sqlExpressionFactory.Function(
            startsWith ? "left" : "right",
            new[]
            {
            uaInstanceExpression,
            _sqlExpressionFactory.Function(
                "length",
                new[] { pattern },
                nullable: true,
                argumentsPropagateNullability: new[] { true, true },
                typeof(int))
            },
            nullable: true,
            argumentsPropagateNullability: new[] { true, true },
            typeof(string),
            stringTypeMapping);

        // LEFT/RIGHT of a citext return a text, so for non-default text mappings we apply an explicit cast.
        if (instance.TypeMapping != _textTypeMapping)
        {
            leftRight = _sqlExpressionFactory.Convert(leftRight, typeof(string), instance.TypeMapping);
        }

        // Also add an explicit cast on the pattern; this is only required because of
        // The following is only needed because of https://github.com/aspnet/EntityFrameworkCore/issues/19120
        SqlFunctionExpression castPattern = SensitiveTransform(pattern.TypeMapping == _textTypeMapping
            ? pattern
            : _sqlExpressionFactory.Convert(pattern, typeof(string), pattern.TypeMapping));

        return startsWith
            ? _sqlExpressionFactory.AndAlso(
                _sqlExpressionFactory.Like(
                    uaInstanceExpression,
                    _sqlExpressionFactory.Add(
                        SensitiveTransform(pattern),
                        _sqlExpressionFactory.Constant("%")),
                    _sqlExpressionFactory.Constant(string.Empty)),
                _sqlExpressionFactory.Equal(leftRight, castPattern))
            : _sqlExpressionFactory.Equal(leftRight, castPattern);
    }

    /// <summary>
    /// Check if an wild character
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static bool IsLikeWildChar(char c)
    {
        return c is '%' or '_';
    }

    /// <summary>
    /// Escape wild character
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    private static string EscapeLikePattern(string pattern)
    {
        StringBuilder builder = new();
        for (int i = 0; i < pattern.Length; i++)
        {
            char c = pattern[i];
            if (IsLikeWildChar(c) || c == LikeEscapeChar)
            {
                builder.Append(LikeEscapeChar);
            }
            builder.Append(c);
        }
        return builder.ToString();
    }

    #endregion

}