namespace Lagoon.UI.Components;

/// <summary>
/// Column for edit mode
/// </summary>
public class LgGridEditColumn : LgGridCommandColumn
{
    #region constants

    /// <summary>
    /// Unique key of the edit column.
    /// </summary>
    private const string DEFAULT_UNIQUE_KEY = "_DEFEDT";

    /// <summary>
    /// Command to edit row mode
    /// </summary>
    internal const string EDITCOMMAND = "Grid-EditRow";

    /// <summary>
    /// Command to save edited row
    /// </summary>
    internal const string SAVECOMMAND = "Grid-SaveRow";

    /// <summary>
    /// Command to save add row
    /// </summary>
    internal const string ADDCOMMAND = "Grid-AddRow";

    /// <summary>
    /// Command to cancel changes in edited row
    /// </summary>
    internal const string EDITCANCELCOMMAND = "Grid-EditCancelRow";

    /// <summary>
    /// Command to cancel changes in add row
    /// </summary>
    internal const string ADDCANCELCOMMAND = "Grid-AddCancelRow";        

    #endregion

    #region methods        

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridEditCell<>).MakeGenericType(itemType);
    }

    ///<inheritdoc/>
    protected override string DefaultUniqueKey()
    {
        return DEFAULT_UNIQUE_KEY;
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        // Disable buttons toolbar edit and add management if edit column exists            
        base.OnInitialized();
        GridView.HasEditColumn = true;
    }

    #endregion

}
