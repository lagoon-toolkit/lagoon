namespace Lagoon.UI.Components;

/// <summary>
/// Gridview numeric column
/// </summary>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public class LgGridNumericColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{

    /// <summary>
    /// Gets or sets the input prefix.
    /// </summary>
    [Parameter]
    public string Prefix { get; set; }

    /// <summary>
    /// Indicate if prefix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType PrefixType { get; set; }

    /// <summary>
    /// Gets or sets the input suffix.
    /// </summary>
    [Parameter]
    public string Suffix { get; set; }

    /// <summary>
    /// Indicate if suffix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType SuffixType { get; set; }

    /// <summary>
    /// Number of decimals to display.
    /// </summary>
    [Parameter]
    public int? Decimals { get; set; }

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridNumericCell<,,>).MakeGenericType(itemType, columnValueType, cellValueType);
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(DisplayFormat))
        {
            Decimals ??= State.CellValueType.GetDefaultDecimalDigits();
            DisplayFormat = $"N{Decimals:D}";
        }
        InputDisplayFormat ??= DisplayFormat;
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(NumericFilter<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgNumericFilterBox<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        State.PrefixCalculation = Prefix;
        State.PrefixCalculationType = PrefixType;
        State.SuffixCalculation = Suffix;
        State.SuffixCalculationType = SuffixType;
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
    }

    #endregion

}