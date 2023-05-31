namespace Lagoon.UI.Components;


/// <summary>
/// Filter description builder.
/// </summary>
public class FilterDescriptionBuilder<TValue>
{

    #region constants

    private const char RANGE_SEPARATOR = '➔';
    private const string SEPARATOR = ", ";

    #endregion

    #region fields

    private StringBuilder _description = new();

    #endregion

    #region properties

    /// <summary>
    /// Return the default methods to use to format values.
    /// </summary>
    /// <returns>The default methods to use to format values.</returns>
    protected Func<TValue, string> FormatValue { get; }

    /// <summary>
    /// Separator between values.
    /// </summary>
    public string Separator { get; set; } = SEPARATOR;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="formatValue">The metod to use to format values.</param>
    /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
    public FilterDescriptionBuilder(Func<TValue, string> formatValue)
    {
        FormatValue = formatValue ?? throw new ArgumentNullException(nameof(formatValue));
    }

    #endregion

    #region methods

    /// <summary>
    /// Add a boolean value to the description.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="addSeparator">Add a separator before (if the description is not empty).</param>
    public void AppendValue(TValue value, bool addSeparator = true)
    {
        AppendSeparator(addSeparator);
        if (value is null)
        {
            _description.Append("FilterEmptyValue".Translate());
        }
        else
        {
            _description.Append(FormatValue(value));
        }
    }

    /// <summary>
    /// Add text to the description.
    /// </summary>
    /// <param name="text"></param>
    public void Append(string text)
    {
        _description.Append(text);
    }

    /// <summary>
    /// Add a separator between range values if the description is not empty.
    /// </summary>
    public void AppendBetweenSeparator()
    {
        _description.Append(RANGE_SEPARATOR);
    }

    /// <summary>
    /// Add a separator between values if the description is not empty.
    /// </summary>
    /// <param name="add">If false, the separator is not added.</param>
    public void AppendSeparator(bool add = true)
    {
        if (add && _description.Length > 0)
        {
            _description.Append(Separator);
        }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <returns>The description.</returns>
    public override string ToString()
    {
        return _description.ToString();
    }

    #endregion

}
