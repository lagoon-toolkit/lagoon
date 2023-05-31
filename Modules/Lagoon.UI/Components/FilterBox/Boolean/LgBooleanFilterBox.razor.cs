using Lagoon.Core.Helpers;

namespace Lagoon.UI.Components;

/// <summary>
/// Filter box to filter bool or bool? values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgBooleanFilterBox<TValue> : LgFilterBoxBase<TValue, BooleanFilter<TValue>>
{

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        return typeof(LgBooleanFilterEditor<TValue>);
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TValue value)
    {
        return BooleanFormatter.Default.Format(value);
    }

    #endregion

}