namespace Lagoon.UI.Components;

/// <summary>
/// Component Tab
/// </summary>
public class LgTab : LgCustomTab
{

    #region parameters

    /// <summary>
    /// Gets or sets if the tab is closable
    /// </summary>
    [Parameter]
    public bool Closable { get; set; }

    /// <summary>
    /// Gets or sets if the tab can be moved
    /// </summary>
    [Parameter]
    public bool Draggable { get; set; } = true;

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override void OnInitTabData(LgTabData tabData)
    {
        base.OnInitTabData(tabData);
        tabData.Closable = Closable;
        tabData.Draggable = Draggable;
        //Zzz            tabData.Pinned = Pinned;
    }

    #endregion

}
