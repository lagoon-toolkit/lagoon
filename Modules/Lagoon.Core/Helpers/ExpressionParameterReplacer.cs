namespace Lagoon.Helpers.Expressions;

/// <summary>
/// Helper to modify the body of a lambda expression.
/// </summary>
public class ExpressionParameterReplacer : ExpressionVisitor
{

    #region fields

    /// <summary>
    /// The old parameter.
    /// </summary>
    private ParameterExpression _oldParameter;

    /// <summary>
    /// The expression to replace the old parameter.
    /// </summary>
    private Expression _newExpression;

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="oldParameter">The old parameter.</param>
    /// <param name="newExpression">The expression to replace the old parameter.</param>
    private ExpressionParameterReplacer(ParameterExpression oldParameter, Expression newExpression)
    {
        _oldParameter = oldParameter;
        _newExpression = newExpression;
    }

    #endregion

    #region methods

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="valueSelector">The lambda expression from which to extract the body.</param>
    /// <param name="newExpression">The expression to replace the parameter expression of the lambda.</param>
    /// <returns></returns>
    public static Expression GetMergeableBody(LambdaExpression valueSelector, Expression newExpression)
    {
        ExpressionParameterReplacer replacer = new(valueSelector.Parameters[0], newExpression);
        return replacer.Visit(valueSelector.Body);
    }

    ///<inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (ReferenceEquals(node, _oldParameter))
        {
            return _newExpression;
        }
        else
        {
            return base.VisitParameter(node);
        }
    }

    ///<inheritdoc/>
    protected override Expression VisitMember(MemberExpression node)
    {
        if (ReferenceEquals(node, _oldParameter))
        {
            return _newExpression;
        }
        else if (ReferenceEquals(node.Expression, _oldParameter))
        {
            return node.Update(_newExpression);
        }
        else
        {
            return base.VisitMember(node);
        }
    }

    #endregion

}