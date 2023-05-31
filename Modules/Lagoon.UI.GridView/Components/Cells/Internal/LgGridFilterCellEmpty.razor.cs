namespace Lagoon.UI.Components.Internal;

/// <summary>
/// En empty cell in the filter line.
/// </summary>
public partial class LgGridFilterCellEmpty : LgGridFilterCellBase
{

    #region methods

    ///<inheritdoc/>
    protected override void OnBuildCssClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildCssClassAttribute(builder);
        builder.Add("gridview-filter-emtpy");
    }

    #endregion

}
