namespace Lagoon.UI.Components.Internal;

/// <summary>
/// En empty cell in the filter line.
/// </summary>
public partial class LgGridFilterCellCustom : LgGridFilterCellBase
{

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    public LgGridFilterCellCustom()
    {
        RenderFilterContent = Column.FilterCellContent;
    }

    #endregion

}
