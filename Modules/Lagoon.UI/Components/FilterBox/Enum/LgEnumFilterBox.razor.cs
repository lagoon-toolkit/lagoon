namespace Lagoon.UI.Components;

/// <summary>
/// Filter box to filter enum values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgEnumFilterBox<TValue> : LgFilterBoxBase<TValue, EnumFilter<TValue>>
{

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        return typeof(LgEnumFilterEditor<TValue>);
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TValue value)
    {
        return value switch
        {
            null => null,
            Enum enumValue => enumValue.GetDisplayName(),
            _ => value.ToString()
        };
    }

    #endregion

}
