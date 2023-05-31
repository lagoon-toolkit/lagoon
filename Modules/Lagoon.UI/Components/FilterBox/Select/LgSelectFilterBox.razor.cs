namespace Lagoon.UI.Components;

/// <summary>
/// Filter box to filter enum values.
/// </summary>
/// <typeparam name="TValue">Base value type</typeparam>    
public partial class LgSelectFilterBox<TValue> : LgFilterBoxBase<TValue, SelectFilter<TValue>>
{

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        return typeof(LgSelectFilterEditor<TValue>);
    }

    #endregion

}
