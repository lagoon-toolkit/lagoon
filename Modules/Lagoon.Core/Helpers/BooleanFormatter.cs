namespace Lagoon.Core.Helpers;

/// <summary>
/// Helper to display text from boolean value.
/// </summary>
public class BooleanFormatter
{

    #region constants

    /// <summary>
    /// The seprator between keys.
    /// </summary>
    private const char SEPARATOR = ';';

    #endregion

    #region fields

    /// <summary>
    /// The default boolean formatter.
    /// </summary>
    private static BooleanFormatter _default;

    #endregion

    #region properties

    /// <summary>
    /// The default boolean formatter ("Yes";"No","-").
    /// </summary>
    public static BooleanFormatter Default
    {
        get
        {
            _default ??= new ("#BooleanTrue", "#BooleanFalse", "#BooleanIndeterminate");
            return _default;
        }
    }

    /// <summary>
    /// Gets the value displayed when the condition is <c>false</c>.
    /// </summary>
    public string FalseDisplayValue { get; }

    /// <summary>
    /// Gets the value displayed when the condition is <c>true</c>.
    /// </summary>
    public string TrueDisplayValue { get; }

    /// <summary>
    /// Gets the value displayed when the condition is <c>null</c>.
    /// </summary>
    public string IndeterminateDisplayValue { get; }

    #endregion

    #region constructors

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("It will be more optimized to use \"BooleanFormatter.Default\" rather than create a new one.")]
    public BooleanFormatter() : this("#BooleanTrue", "#BooleanFalse", "#BooleanIndeterminate"){}

    /// <summary>
    /// Create a new boolean formatter with the specified values.
    /// </summary>
    /// <param name="displayFormat">The values, separated by semicolumn. Ex: "Yes;No;-"</param>
    public BooleanFormatter(string displayFormat)
    {
        string[] values = displayFormat.Split(SEPARATOR);
        if (values.Length < 2)
        {
            throw new ArgumentException($"The '{SEPARATOR}' must be used to separate values.", nameof(displayFormat));
        }
        TrueDisplayValue = values[0];
        FalseDisplayValue = values[1];
        if (values.Length > 2)
        {
            IndeterminateDisplayValue = values[2];
        }
    }

    /// <summary>
    /// Create a new boolean formatter with the specified values.
    /// </summary>
    /// <param name="trueDisplayValue">The value displayed when the condition is <c>true</c>.</param>
    /// <param name="falseDisplayValue">The value displayed when the condition is <c>false</c>.</param>
    /// <param name="indeterminateDisplayValue">The value displayed when the condition is <c>null</c>.</param>
    public BooleanFormatter(string trueDisplayValue, string falseDisplayValue, string indeterminateDisplayValue)
    {
        FalseDisplayValue = falseDisplayValue;
        TrueDisplayValue = trueDisplayValue;
        IndeterminateDisplayValue = indeterminateDisplayValue;
    }

    #endregion

    #region methods

    /// <summary>
    /// Format the boolean valuer.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>The text representing the boolean value.</returns>
    public string Format(bool value)
    {
        return (value ? TrueDisplayValue : FalseDisplayValue).CheckTranslate();
    }

    /// <summary>
    /// Format the boolean valuer.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>The text representing the boolean value.</returns>
    public string Format(bool? value)
    {
        if (value.HasValue)
        {
            return Format(value.Value);
        }
        else
        {
            return IndeterminateDisplayValue.CheckTranslate();
        }
    }

    /// <summary>
    /// Format the boolean valuer.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>The text representing the boolean value.</returns>
    public string Format<T>(T value)
    {
        if (value is bool boolValue)
        {
            return Format(boolValue);
        }
        else
        {
            return IndeterminateDisplayValue.CheckTranslate();
        }
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{TrueDisplayValue};{FalseDisplayValue};{IndeterminateDisplayValue}";
    }

    #endregion

}
