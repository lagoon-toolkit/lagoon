namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Column that is not bound to a value of an item.
/// </summary>
public abstract class LgGridBaseNoValueColumn : LgGridBaseColumn
{

    #region cell render fragments

    /// <summary>
    /// Gets or sets template of the normal cell
    /// </summary>
    [Parameter]
    public RenderFragment CellContent { get; set; }

    /// <summary>
    /// Gets or sets template of the edit cell
    /// </summary>
    [Parameter]
    public RenderFragment EditCellContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override RenderFragment GetCustomCellContent()
    {
        return CellContent;
    }

    ///<inheritdoc/>
    internal override RenderFragment GetCustomEditCellContent()
    {
        return EditCellContent;
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return null;
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return null;
    }

    #endregion

}
