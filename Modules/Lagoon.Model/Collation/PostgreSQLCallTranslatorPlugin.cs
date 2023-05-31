using Lagoon.Helpers;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lagoon.Model.Collation;

/// <summary>
/// 
/// </summary>
public class PostgreSQLCallTranslatorPlugin : IMethodCallTranslatorPlugin
{

    #region fields

    /// <summary>
    /// SQL Expression factory
    /// </summary>
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly CollationType _collationType;
    private readonly IModel _model;
    private readonly ITypeMappingSource _typeMappingSource;

    #endregion

    #region constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sqlExpressionFactory"></param>
    /// <param name="model"></param>
    /// <param name="typeMappingSource"></param>
    /// <param name="collationType"></param>
    public PostgreSQLCallTranslatorPlugin(ISqlExpressionFactory sqlExpressionFactory,
        IModel model, ITypeMappingSource typeMappingSource, CollationType collationType)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _collationType = collationType;
        _model = model;
        _typeMappingSource = typeMappingSource;
    }

    #endregion

    #region methods

    /// <summary>
    /// Gets translators
    /// </summary>
    public IEnumerable<IMethodCallTranslator> Translators => new IMethodCallTranslator[] {
        new PostgreSQLCallTranslator(_sqlExpressionFactory, _model, _typeMappingSource, _collationType)
    };

    #endregion

}
