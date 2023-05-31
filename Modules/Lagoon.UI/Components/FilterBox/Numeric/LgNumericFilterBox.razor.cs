namespace Lagoon.UI.Components;

/// <summary>
/// Filter box to filter numeric values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgNumericFilterBox<TValue> : LgFilterBoxBase<TValue, NumericFilter<TValue>>
{

    #region fields

    private string _defaultFormat;

    #endregion

    #region properties

    /// <summary>
    /// The default format.
    /// </summary>
    private string DefaultFormat {
        get
        {
            _defaultFormat ??= $"N{typeof(TValue).GetDefaultDecimalDigits():D}";
            return _defaultFormat;
        }
    }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgNumericFilterBox()
    {
        ActiveTabs = FilterTab.Selection | FilterTab.Rules;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        Type type = typeof(TValue);
        Type nullableType = Nullable.GetUnderlyingType(type) is null ? typeof(Nullable<>).MakeGenericType(type) : type;
        return typeof(LgNumericFilterEditor<,>).MakeGenericType(type, nullableType);
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TValue value)
    {
        return value switch
        {
            null => null,
            IFormattable numericValue => numericValue.ToString(DefaultFormat, null),
            _ => value.ToString()
        };
    }

    #endregion

}
