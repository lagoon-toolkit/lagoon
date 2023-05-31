namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Filterbox for text data
/// </summary>
public partial class LgTextFilterBox : LgFilterBoxBase<string, TextFilter>
{

    #region parameter

    /// <summary>
    /// Parameters for the inputmask.
    /// </summary>
    [Parameter]
    public InputMaskOptions InputMaskOptions { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgTextFilterBox()
    {
        ActiveTabs = FilterTab.Selection | FilterTab.Rules;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override Type GetFilterEditorComponentType()
    {
        return typeof(LgTextFilterEditor);
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(string value)
    {
        return value;
    }

    #endregion

}
