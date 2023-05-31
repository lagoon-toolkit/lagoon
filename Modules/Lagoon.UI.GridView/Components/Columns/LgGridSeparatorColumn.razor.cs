namespace Lagoon.UI.Components;

/// <summary>
/// Separator column
/// </summary>
public class LgGridSeparatorColumn : LgGridBaseNoValueColumn
{

    #region fields

    /// <summary>
    /// Unique key of the separator column
    /// </summary>
    private const string DEFAULT_UNIQUE_KEY = "_SEPARATOR";

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridSeparatorCell<>).MakeGenericType(itemType);
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
        State.Resizable = false;
        State.DefaultWidth = "50px";
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
        State.Visible = true;
    }

    #endregion

}
