using Lagoon.Helpers.Expressions;

namespace Lagoon.Helpers;

/// <summary>
/// The filter associated to a value in a model.
/// </summary>
public class ValueFilter
{

    #region properties

    /// <summary>
    /// The filter to apply to the value.
    /// </summary>
    public Filter Filter { get; set; }

    /// <summary>
    /// The selector to get the value.
    /// </summary>
    [JsonIgnore]
    public LambdaExpression Selector { get; }

    /// <summary>
    /// Gets or sets the selector string formated
    /// </summary>
    public string StringifiedSelector { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="selector">The selector.</param>
    /// <param name="filter">The filter informations.</param>
    public ValueFilter(LambdaExpression selector, Filter filter)
    {
        Selector = selector;
        StringifiedSelector = selector.ToString();
        Filter = filter;
    }

    /// <summary>
    /// Unserializer constructor
    /// </summary>
    /// <param name="stringifiedSelector"></param>
    /// <param name="filter"></param>
    [JsonConstructor]
    public ValueFilter(string stringifiedSelector, Filter filter)
    {
        StringifiedSelector = stringifiedSelector;
        Filter = filter;
    }

    #endregion

    #region methods

    /// <summary>
    /// Gets the conditions expressions for the filter.
    /// </summary>
    /// <param name="parameter">The parameter to use to get the body expression.</param>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The conditions expressions for the filter.</returns>
    internal IEnumerable<Expression> GetWhereConditionExpressions(ParameterExpression parameter, FilterWhereContext context)
    {
        if (Filter is not null)
        {
            Expression selector = ExpressionParameterReplacer.GetMergeableBody(Selector, parameter);                                
            if (Filter.IsNumeric && Selector.ReturnType.IsEnum)
            {                    
                selector = Expression.Convert(selector, Selector.ReturnType.GetEnumUnderlyingType());
            }                
            foreach (LambdaExpression lambda in Filter.GetAndExpressions(context))
            {
                yield return ExpressionParameterReplacer.GetMergeableBody(lambda, selector);
            }
        }
    }

    #endregion

}
