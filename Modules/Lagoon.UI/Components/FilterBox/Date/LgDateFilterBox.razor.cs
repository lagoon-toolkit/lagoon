namespace Lagoon.UI.Components;

/// <summary>
/// Filter box for date
/// </summary>    
public partial class LgDateFilterBox<TDateTime> : LgFilterBoxBase<TDateTime, DateFilter<TDateTime>>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgDateFilterBox()
    {
        ActiveTabs = FilterTab.Selection | FilterTab.Rules;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        return typeof(LgDateFilterEditor<TDateTime>);
    }

    #endregion

}
