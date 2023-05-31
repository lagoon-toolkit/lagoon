using Lagoon.UI.Application;
using Lagoon.UI.Components.Input.Internal;
using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;


/// <summary>
/// Grid cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellValue">The type of value bound to the cell.</typeparam>
public abstract partial class LgGridCell<TItem, TColumnValue, TCellValue> : LgGridBaseCell<TItem> where TCellValue : TColumnValue
{

    #region fields                

    private Func<TCellValue> _getCellValue;

    /// <summary>
    /// Indicate if cell modification is valide
    /// </summary>
    protected bool _validated = true;

    /// <summary>
    /// Indicate the previous row editing state
    /// </summary>
    protected bool _lastRowEditState;

    /// <summary>
    /// Previous edited value
    /// </summary>
    protected TCellValue _previousValue;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets row of the cell
    /// </summary>
    [CascadingParameter]
    protected EditContext EditContext { get; set; }

    #endregion

    #region parameters                        

    /// <summary>
    /// Gets or sets value
    /// </summary>
    [Obsolete("You should use the \"CellValue\" property.")]
    [Parameter]
    public virtual TColumnValue ColumnValue { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Obsolete("You should use the \"CellValueExpression\" property.")]
    [Parameter]
    public Expression<Func<TColumnValue>> ColumnValueExpression { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Obsolete("You should use the \"CellValueChanged\" property.")]
    [Parameter]
    public EventCallback<TColumnValue> ColumnValueChanged { get; set; }

    /// <summary>
    /// Current value for the cell.
    /// </summary>
    public TCellValue CellValue => _getCellValue();

    /// <summary>
    /// Event raised when the value is updated by the user.
    /// </summary>
    public EventCallback<TCellValue> CellValueChanged { get; private set; }

    /// <summary>
    /// Expression representing the bound property.
    /// </summary>
    public LambdaExpression CellValueExpression { get; private set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets type of component in edit cell
    /// </summary>
    internal virtual Type InputType => typeof(LgTextBox);

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Map the Value... properties to the orgininal properties
#pragma warning disable CS0618 // (Fake Obsolete) : This is the only place where "ColumnValue..." properties must be called
        _getCellValue = () => (TCellValue)ColumnValue;
        CellValueChanged = EventCallback.Factory.Create<TCellValue>(this,
                __value => ColumnValueChanged.TryInvokeAsync(Column.GridView.App, __value));
        if (typeof(TColumnValue) == typeof(TCellValue))
        {
            // We keep the original expression to acces to validation attributes
            CellValueExpression = ColumnValueExpression;
        }
        else
        {
            CellValueExpression = (Expression<Func<TCellValue>>)(() => CellValue);
        }
#pragma warning restore CS0618
        _lastRowEditState = RowEditing;
        InputBusyReference = new InputBusyReference { ExpectedInputType = InputType };
        if (Column is LgGridBaseValueColumn<TColumnValue> column && column.CustomEditType is not null)
        {
            InputBusyReference.ExpectedInputType = column.CustomEditType;
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Column span
        if (Row.ActiveColSpan > 0)
        {
            // Remove colspan for the next cells                    
            Style = "display: none;";
            Row.ActiveColSpan--;
        }
        else if (Column.ColSpan > 0)
        {
            Style = $"grid-column: {Column.State.Order} / {Column.State.Order + Column.ColSpan};";
            Row.ActiveColSpan = Column.ColSpan - 1;
        }
        // Manage edit row mode cancellation
        if (GridView.EditMode == GridEditMode.Row && _lastRowEditState != RowEditing)
        {
            _lastRowEditState = RowEditing;
            Row.RowEditingCancellation?.Remove(CancelEditAsync);
            // Keep value before change
            if (RowEditing)
            {
                Row.RowEditingCancellation.Add(CancelEditAsync);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        if (IsInEditMode)
        {
            builder.Add("gridview-cell-edit");
        }
        if (CanEdit && GridView.EditMode == GridEditMode.Cell)
        {
            builder.Add("gridview-cell-inline-editable");
        }
    }

    /// <summary>
    /// Gets the content of the display cell
    /// </summary>
    /// <returns></returns>        
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", GridView.WrapContent ? "" : "text-truncate");
            builder.AddContent(2, FormatValueAsString(CellValue));
            builder.CloseElement();
        };
    }

    /// <summary>
    /// Return value formated to be displayed
    /// </summary>
    /// <returns></returns>
    protected virtual string FormatValueAsString(TCellValue cellValue)
    {
        return cellValue is IFormattable value ? value.ToString(Column.DisplayFormat, null) : (cellValue?.ToString());
    }

    /// <summary>
    /// Gets the content of the edit cell
    /// </summary>
    /// <returns></returns>
    protected override RenderFragment GetEditContent()
    {
        return builder =>
        {
            builder.OpenComponent(0, InputType);
            builder.AddAttribute(1, nameof(LgInputRenderBase<TCellValue>.Value), CellValue);
            builder.AddAttribute(2, nameof(LgInputRenderBase<TCellValue>.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgInputRenderBase<TCellValue>.ValueChanged), CellValueChanged);
            builder.AddAttribute(4, nameof(LgInputRenderBase<TCellValue>.OnChange), EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));
            builder.AddAttribute(5, nameof(LgInputRenderBase<TCellValue>.ConfirmationMessage), Column.Confirmation);
            Func<TCellValue, TCellValue, Task<bool>> onValueChange = OnValueChange;
            builder.AddAttribute(6, nameof(LgInputRenderBase<TCellValue>.ValueChangeCallback), onValueChange);
            builder.AddAttribute(7, nameof(LgInputRenderBase<TCellValue>.ResetButton), !GridViewBehaviour.Options.ResetInputConfiguration.HideAlwaysResetButton);
            builder.AddAttribute(8, nameof(LgInputRenderBase<TCellValue>.ResetText), GridViewBehaviour.Options.ResetInputConfiguration.ResetText);
            builder.AddAttribute(9, nameof(LgInputRenderBase<TCellValue>.ResetTextAriaLabel), GridViewBehaviour.Options.ResetInputConfiguration.ResetTextAriaLabel);
            builder.AddAttribute(10, nameof(LgInputRenderBase<TCellValue>.UseResetGridViewConfiguration), true);

            // Add the InputMask attributes
            builder.AddMultipleAttributes(11, GetInputAdditionalAttributes());
            builder.AddComponentReferenceCapture(12, cp => InputBusyReference.Reference = (IInputBusy)cp);
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Method call before OnChange callback
    /// </summary>
    /// <param name="value"></param>
    /// <param name="previousValue"></param>
    /// <returns></returns>
    protected Task<bool> OnValueChange(TCellValue value, TCellValue previousValue)
    {
        _previousValue = previousValue;
        return Column is LgGridBaseValueColumn<TColumnValue> column
            ? column.OnValueChangeAsync(Row.Item, value, previousValue)
            : Task.FromResult(true);
    }

    /// <summary>
    /// Get the additional attribute to add to the input box.
    /// </summary>
    /// <returns>The list of attributes to add.</returns>
    protected virtual IEnumerable<KeyValuePair<string, object>> GetInputAdditionalAttributes()
    {
        if (Column.InputMaskOptions is not null && InputType == typeof(LgTextBox))
        {
            return Column.InputMaskOptions.EnumerateAttributes();
        }
        else
        {
            Dictionary<string, object> attributes = new();
            LoadInputAdditionalAttributes(attributes);
            return attributes.Count == 0 ? null : attributes;
        }
    }

    /// <summary>
    /// Load the additional attribute to add to the input box.
    /// </summary>
    /// <returns>The list of attributes to add.</returns>
    protected virtual void LoadInputAdditionalAttributes(Dictionary<string, object> attributes)
    { }

    /// <summary>
    ///  Detect if the cell has been modify
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task SafeOnChangeAsync(ChangeEventArgs args)
    {
        try
        {
            await OnChangeAsync(args);
        }
        catch (Exception ex)
        {
            GridView.ShowException(ex);
        }
    }

    /// <summary>
    ///  Detect if the cell has been modify
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async Task OnChangeAsync(ChangeEventArgs args)
    {
        if (Column is LgGridBaseValueColumn<TColumnValue> column)
        {
            // Update event in inline mode            
            bool cellEditMode = GridView.EditMode == GridEditMode.Cell && !IsAdd;
            if (cellEditMode)
            {
                await column.OnPatchValueAsync(Row.Item, args.Value, _previousValue);
            }
            Row.Refresh();
        }
        await Task.CompletedTask;
    }

    ///<inheritdoc/>
    protected override Task OnEnterEditModeAsync()
    {
        // Change the cell state
        return base.OnEnterEditModeAsync();
    }

    ///<inheritdoc/>
    protected override async Task OnLeaveEditModeAsync(bool cancelEdition)
    {
        if (InputBusyReference.Reference is null || !InputBusyReference.Reference.IsBusy)
        {
            if (cancelEdition)
            {
                Row.ValidateEditContext();
                _validated = true;
            }
            if (_validated)
            {
                // Restore the readonly mode                                   
                await base.OnLeaveEditModeAsync(cancelEdition);
            }
        }
    }

    /// <summary>
    /// Cancel edited value
    /// </summary>
    /// <returns></returns>
    private Task CancelEditAsync()
    {
        return InputBusyReference.Reference is null
            ? Task.CompletedTask
            : InputBusyReference.Reference.CancelValueAsync();
    }

    ///<inheritdoc/>
    public override void Dispose()
    {
        InputBusyReference.Reference = null;
        Row?.RowEditingCancellation?.Remove(CancelEditAsync);
        base.Dispose();
    }

    #endregion
}
