using System.Runtime.CompilerServices;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// NumericFilterBox filter value
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TNullableValue"></typeparam>
internal class FilterNumericValue<TValue, TNullableValue>
{
    #region properties

    /// <summary>
    /// Filter operator
    /// </summary>
    public FilterNumericOperator FilterOperator { get; set; }

    /// <summary>
    /// Filter value
    /// </summary>
    public TNullableValue Value { get; set; }

    /// <summary>
    /// Allowed filter operator
    /// </summary>
    public IEnumerable<FilterNumericOperator> AllowedOperators { get; private set; }

    #endregion

    #region constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public FilterNumericValue()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filterNumericOperator"></param>
    public FilterNumericValue(TValue value, FilterNumericOperator filterNumericOperator)
    {
        Value = ToTNullableValue(value);
        FilterOperator = filterNumericOperator;
    }

    /// <summary>
    /// Constructor with limited operator
    /// </summary>
    public FilterNumericValue(IEnumerable<FilterNumericOperator> allowedOperator)
    {
        AllowedOperators = allowedOperator;
        FilterOperator = AllowedOperators.First();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filterNumericOperator"></param>
    /// <param name="allowedOperator"></param>
    public FilterNumericValue(TValue value, FilterNumericOperator filterNumericOperator, IEnumerable<FilterNumericOperator> allowedOperator)
    {
        Value = ToTNullableValue(value);
        FilterOperator = filterNumericOperator;
        AllowedOperators = allowedOperator;
    }

    //TODO display selection

    //TODO Build filter on validation

    #endregion

    #region methods

    private static TNullableValue ToTNullableValue(TValue value)
    {
        // We don't use boxing
        if (typeof(TNullableValue) == typeof(TValue))
        {
            return Unsafe.As<TValue, TNullableValue>(ref value);
        }
        // We use the boxing
        return (TNullableValue)(object)value;
    }

    public TValue GetTValue()
    {
        TNullableValue value = Value;
        // We don't use boxing
        if (typeof(TNullableValue) == typeof(TValue))
        {
            return Unsafe.As<TNullableValue, TValue>(ref value);
        }
        // We use the boxing
        return (TValue)(object)value;
    }

    #endregion

}
