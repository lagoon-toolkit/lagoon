using Lagoon.Helpers.Expressions;

namespace Lagoon.UI.Components;


/// <summary>
/// Filter parameter
/// </summary>
public class FilterItem<TValue>
{

    #region properties

    /// <summary>
    /// List of values to include in the filtred list.
    /// </summary>
    [JsonPropertyName("val")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TValue> Values { get; set; }

    /// <summary>
    /// Gets or sets if the filter value is exclude. By default, the value is included.
    /// </summary>
    [JsonPropertyName("not")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Exclude { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    public IEnumerable<LambdaExpression> GetWhereExpressions(FilterWhereContext context)
    {
        if (Exclude)
        {
            return GetWhereExcludeExpressions(context);
        }
        else
        {
            return GetWhereIncludeExpressions(context);
        }
    }

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    protected virtual IEnumerable<LambdaExpression> GetWhereIncludeExpressions(FilterWhereContext context)
    {
        if (Values is not null && Values.Count > 0)
        {
            switch (Values.Count)
            {
                case 0:
                    break;
                case 1:
                    yield return GetWhereFirstValueFunction();
                    break;
                default:
                    yield return GetWhereInFunction();
                    break;
            }
        }
    }

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    private IEnumerable<LambdaExpression> GetWhereExcludeExpressions(FilterWhereContext context)
    {
        IEnumerable<LambdaExpression> include = GetWhereIncludeExpressions(context);
        Expression where = null;
        ParameterExpression parameter = null;
        foreach (LambdaExpression lambda in include)
        {
            if (where is null)
            {
                parameter = lambda.Parameters[0];
                where = lambda.Body;
            }
            else
            {
                where = Expression.AndAlso(where, ExpressionParameterReplacer.GetMergeableBody(lambda, parameter));
            }
        }
        if (where is not null)
        {
            yield return Expression.Lambda(ApplyNotExpression(where), parameter);
        }
    }

    /// <summary>
    /// Add a "not" to the body expression of the lambda expression if the "Exclude" property is true. 
    /// </summary>
    /// <param name="body">The condition expression.</param>
    /// <returns>The negatiion of the condition expression.</returns>
    private static Expression ApplyNotExpression(Expression body)
    {
        BinaryExpression binaryExpression;
        switch (body.NodeType)
        {
            case ExpressionType.Not:
                return ((UnaryExpression)body).Operand;
            case ExpressionType.NotEqual:
                binaryExpression = (BinaryExpression)body;
                return Expression.Equal(binaryExpression.Left, binaryExpression.Right, binaryExpression.IsLiftedToNull, binaryExpression.Method);
            case ExpressionType.Equal:
                binaryExpression = (BinaryExpression)body;
                return Expression.NotEqual(binaryExpression.Left, binaryExpression.Right, binaryExpression.IsLiftedToNull, binaryExpression.Method);
            default:
                return Expression.Not(body);
        }
    }

    /// <summary>
    /// The expression representing the "where in list" condition.
    /// </summary>
    /// <returns>The expression representing the "where in list" condition.</returns>
    private Expression<Func<TValue, bool>> GetWhereFirstValueFunction()
    {
        // x=> x == Values[0]
        ParameterExpression xParameter = Expression.Parameter(typeof(TValue), "x");
        ConstantExpression valueExpression = Expression.Constant(Values[0], typeof(TValue));
        Expression bodyExpression = Expression.Equal(xParameter, valueExpression);
        return Expression.Lambda<Func<TValue, bool>>(bodyExpression, xParameter);
    }

    /// <summary>
    /// The expression representing the "where in list" condition.
    /// </summary>
    /// <returns>The expression representing the "where in list" condition.</returns>
    private Expression<Func<TValue, bool>> GetWhereInFunction() //x FilterWhereContext context)
    {
        //if (context)
        //{
        //    //AsEnumerable is required for ODATA : https://github.com/OData/odata.net/issues/2011
        //    return x => Values.AsEnumerable().Contains(x);
        //}
        //else
        //{
        return x => Values.Contains(x);
        //}
    }

    #endregion

}
