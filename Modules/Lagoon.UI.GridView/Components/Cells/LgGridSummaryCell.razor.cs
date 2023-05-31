using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Summary cell
/// </summary>
/// <typeparam name="TItem">The type of data in the list bound to the grid.</typeparam>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
/// <typeparam name="TCellData">The type of value bound to the cell.</typeparam>
public class LgGridSummaryCell<TItem, TColumnValue, TCellData> : LgGridCell<TItem, TColumnValue, TCellData> where TCellData : TColumnValue
{

    #region fields

    /// <summary>
    /// Cell reference
    /// </summary>
    private ElementReference _cellReference;

    /// <summary>
    /// Indicate if the modal is opened
    /// </summary>
    private bool _modalOpened;

    #endregion        

    #region parameters

    /// <summary>
    /// Gets or sets the maximum number of characters entered
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    #endregion

    #region properties

    /// <inheritdoc/>
    internal override Type InputType => typeof(LgSummaryBox);

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void BuildCell(int index, RenderTreeBuilder builder)
    {
        var startIndex = index++;
        builder.AddElementReferenceCapture(startIndex, inst => _cellReference = inst);
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
            builder.AddAttribute(1, nameof(LgSummaryBox.Value), CellValue);
            builder.AddAttribute(2, nameof(LgSummaryBox.ValueExpression), CellValueExpression);
            builder.AddAttribute(3, nameof(LgSummaryBox.ValueChanged), CellValueChanged);
            builder.AddAttribute(4, nameof(LgSummaryBox.OnChange), EventCallback.Factory.Create<ChangeEventArgs>(this, SafeOnChangeAsync));
            builder.AddAttribute(5, nameof(LgSummaryBox.OnClose), EventCallback.Factory.Create(this, OnCloseAsync));
            builder.AddAttribute(7, nameof(LgSummaryBox.MaxLength), MaxLength);
            builder.AddAttribute(8, nameof(LgSummaryBox.TextareaHeight), (Column as LgGridSummaryColumn<string>).TextareaHeight);
            builder.AddAttribute(9, nameof(LgSummaryBox.ModalTitle), Column.State.Title);
            builder.AddAttribute(10, nameof(LgSummaryBox.OnOpen), EventCallback.Factory.Create(this, OnOpen));
            builder.AddAttribute(11, nameof(LgSummaryBox.WrapContent), GridView.WrapContent);
            builder.AddAttribute(12, nameof(LgSummaryBox.ConfirmationMessage), Column.Confirmation);
            Func<TCellData, TCellData, Task<bool>> onValueChange = OnValueChange;
            builder.AddAttribute(13, nameof(LgSummaryBox.ValueChangeCallback), onValueChange);
            builder.AddComponentReferenceCapture(14, cp => InputBusyReference.Reference = (IInputBusy)cp);
            builder.CloseComponent();
        };
    }

    ///<inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        if(GridView.EditMode == GridEditMode.Cell)
        {
            return GetEditContent();
        }
        return base.GetCellContent();
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-summary");
        if (GridView.EditMode == GridEditMode.Cell && !IsInEditMode)
        {
            builder.Add("gridview-cell-edit");
        }
        if (GridView.WrapContent && GridView.EditMode == GridEditMode.Row && !IsInEditMode)
        {
            builder.Add("gridview-col-summary-wrapped");
        }
        if (_modalOpened)
        {
            builder.Add("modal-open");
        }
    }
    
    /// <summary>
    /// Action on open modal
    /// </summary>
    private void OnOpen()
    {
        _modalOpened = true;
    }

    /// <summary>
    /// Leave cell editing on modal close
    /// </summary>
    /// <returns></returns>
    private async Task OnCloseAsync()
    {
        _modalOpened = false;
        // Focus cell after closing
        // On closing the modal a focus is put on an element after 100ms
        // Set delay to 200ms to have focus on cell
        await GridView.JS.FocusAsync(_cellReference, 200);
    }

    ///<inheritdoc/>
    protected override Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Code != "Escape")
        {
            return base.OnKeyDownAsync(args);
        }
        return Task.CompletedTask;
    }

    #endregion
}
