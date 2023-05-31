namespace Lagoon.UI.Components;

/// <summary>
/// Selection column
/// </summary>    
public partial class LgGridSelectionColumn : LgGridBaseNoValueColumn
{

    #region fields

    /// <summary>
    /// Unique key of the command column
    /// </summary>
    private const string DEFAULT_UNIQUE_KEY = "_SELECTION";

    /// <summary>
    /// Command to delete selected rows
    /// </summary>
    internal const string DELETE_SELECTION_COMMAND = "Grid-DeleteSelection";

    #endregion

    #region Parameters
    /// <summary>
    /// Option to disable the selection of all rows in the Gridview
    /// </summary>
    [Parameter]
    public bool DisableSelectAll { get; set; }
    #endregion

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridSelectionCell<>).MakeGenericType(itemType);
    }

    ///<inheritdoc/>
    protected override string OnGetQuickKey(ParameterView parameters)
    {
        return base.OnGetQuickKey(parameters) ?? DEFAULT_UNIQUE_KEY;
    }

    ///<inheritdoc/>
    protected override string OnGetUniqueKey()
    {
        return base.OnGetUniqueKey() ?? DEFAULT_UNIQUE_KEY;
    }

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        State.AllowFilter = false;
        State.AllowSort = false;
        State.AllowHide = false;
        State.DefaultWidth = "30px";
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
        State.Visible = true;
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        // Enable multiple line selection            
        base.OnInitialized();
        GridView.HasSelectionColumn = true;
        CanAdd = false;
    }

    /// <summary>
    /// Select all lines in the Gridview.
    /// </summary>
    private Task HeaderSelectionChangedAsync()
    {
        State.HeaderSelectionState = true;
        return GridView.OnChangeAllSelectionAsync(State.HeaderSelectionState);
    }

    #endregion        
}
