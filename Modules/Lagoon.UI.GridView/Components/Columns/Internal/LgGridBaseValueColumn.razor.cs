namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Base grid column
/// </summary>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public abstract class LgGridBaseValueColumn<TColumnValue> : LgGridBaseColumn
{
    #region fields

    private string _bodyExpression;

    #endregion

    #region cell parameters

    /// <summary>
    /// Gets or sets bound value
    /// </summary>
    [Parameter]
    public TColumnValue Value { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value
    /// </summary>
    [Parameter]
    public EventCallback<TColumnValue> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value
    /// </summary>
    [Parameter]
    public Expression<Func<TColumnValue>> ValueExpression { get; set; }

    #endregion

    #region cell render fragments

    /// <summary>
    /// Gets or sets template of the normal cell
    /// </summary>
    [Parameter]
    public RenderFragment<TColumnValue> CellContent { get; set; }

    /// <summary>
    /// Gets or sets template of the edit cell
    /// </summary>
    [Parameter]
    public RenderFragment<TColumnValue> EditCellContent { get; set; }

    /// <summary>
    /// Gets or sets type of component used in edit mode
    /// </summary>
    [Parameter]
    public Type CustomEditType { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The type of value bound to the column.
    /// </summary>
    internal override Type ColumnValueType => typeof(TColumnValue);

    /// <summary>
    /// Gets if the column has sort expression
    /// </summary>
    internal bool HasSortExpression => State.GetSortExpression() is not null;

    /// <summary>
    /// Gets if the column has filter expression
    /// </summary>
    internal bool HasFilterExpression => State.GetFilterExpression() is not null;

    #endregion

    #region methods       

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        // Disable properties if not binded and not have value or sort expression            
        State.AllowSort = HasSortExpression;
        State.AllowFilter = HasFilterExpression;
        if (ValueExpression is null)
        {
            State.CalculationType = DataCalculationType.None;
        }
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
    }

    /// <summary>
    /// Returns the working type for the cell.
    /// </summary>
    /// <returns>The working type for the cell.</returns>
    /// <remarks>Used because for the <see cref="LgDynamicGridView"/>, the TColumnValue is always <see cref="object"/></remarks>
    internal override Type OnGetCellValueType()
    {
        return GridView.OnGetCellValueType(ColumnValueType, ValueExpression);
    }

    ///<inheritdoc/>
    protected override string OnGetQuickKey(ParameterView parameters)
    {
        _bodyExpression = parameters.GetValueOrDefault<Expression<Func<TColumnValue>>>(nameof(ValueExpression))?.Body?.ToString();
        return base.OnGetQuickKey(parameters) ?? _bodyExpression;
    }

    ///<inheritdoc/>
    protected override string OnGetUniqueKey()
    {
        State.SetParameterizedValueExpression(ValueExpression);
        string unsafeKey = GridView.BuildColumnKey(ValueExpression, State.ParameterizedValueExpression);
        string cleanKey = CleanUniqueKey(unsafeKey);
        State.EditContextFieldName = unsafeKey == cleanKey ? cleanKey : null;
        return base.OnGetUniqueKey() ?? cleanKey;
    }

    ///<inheritdoc/>
    internal override RenderFragment GetCustomCellContent()
    {
        return CellContent is null ? null : CellContent(Value);
    }

    ///<inheritdoc/>
    internal override RenderFragment GetCustomEditCellContent()
    {
        return EditCellContent is null ? null : EditCellContent(Value);
    }

    /// <inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            builder.OpenComponent(1, State.CellComponentType);
            builder.AddAttribute(2, nameof(LgGridBaseCell<object>.CssClass), CssClass);
#pragma warning disable CS0618 // This is the only place where "ColumnValue..." properties must be called
            builder.AddAttribute(3, nameof(LgGridCell<object, TColumnValue, TColumnValue>.ColumnValue), Value);
            builder.AddAttribute(4, nameof(LgGridCell<object, TColumnValue, TColumnValue>.ColumnValueExpression), ValueExpression);
            builder.AddAttribute(5, nameof(LgGridCell<object, TColumnValue, TColumnValue>.ColumnValueChanged), ValueChanged);
#pragma warning restore CS0618
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Event raised when before cell value changed.
    /// </summary>
    /// <param name="item">The item bound to the row.</param>
    /// <param name="value">The new value.</param>
    /// <param name="previousValue">The old value.</param>
    internal Task<bool> OnValueChangeAsync(object item, TColumnValue value, TColumnValue previousValue)
    {
        return GridView.OnValueChangedAsync(item, State.UniqueKey, value, previousValue);
    }

    /// <summary>
    /// Event raised when cell value changed.
    /// </summary>
    /// <param name="item">The item bound to the row.</param>
    /// <param name="value">The new value.</param>
    /// <param name="previousValue">The old value.</param>
    /// <returns></returns>
    internal Task OnPatchValueAsync(object item, object value, TColumnValue previousValue)
    {
        return GridView.OnPatchValueAsync(item, State.UniqueKey, value, previousValue);
    }

    /// <summary>
    /// Return the string representing the value.
    /// </summary>
    /// <typeparam name="TCellValue">The type of value for the column.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The string representing the value.</returns>
    internal virtual string FormatValueAsString<TCellValue>(TCellValue value)
    {
        if (value is IFormattable formatableValue)
        {
            return formatableValue.ToString(DisplayFormat, null);
        }
        else
        {
            return value?.ToString();
        }
    }

    ///<inheritdoc/>
    protected override Type GetFilterCellType()
    {
        if (State.FilterType is not null && State.AllowFilter)
        {
            return typeof(LgGridFilterCellValue<,>).MakeGenericType(typeof(TColumnValue), State.CellValueType);
        }
        else
        {
            return base.GetFilterCellType();
        }
    }

    #endregion
}
