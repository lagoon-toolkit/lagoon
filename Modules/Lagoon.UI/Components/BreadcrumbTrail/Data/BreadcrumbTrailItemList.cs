namespace Lagoon.UI.Components;

/// <summary>
/// List of menu item.
/// </summary>
public class BreadcrumbTrailItemList : List<BreadcrumbTrailItem>
{

    #region properties

    /// <summary>
    /// Get breadcrumb items.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Remove the call to Items property and use method directly on the BreadcrumbTrailItemList instance.")]
    public BreadcrumbTrailItemList Items => this;

    #endregion

    #region methods

    /// <summary>
    /// Add a new menu item into the items collection
    /// </summary>
    public BreadcrumbTrailItem Add(LgPageLink link)
    {
        var item = new BreadcrumbTrailItem() { Link = link };
        base.Add(item);
        return item;
    }

    /// <summary>
    /// Add a new menu item into the items collection
    /// </summary>
    public BreadcrumbTrailItem Add(string text, string iconName = null)
    {
        var item = new BreadcrumbTrailItem() { Text = text, IconName = iconName };
        base.Add(item);
        return item;
    }

    /// <summary>
    /// Add a new menu item into the items collection
    /// </summary>
    public BreadcrumbTrailItem AddUri(string uri, string text, string iconName = null)
    {
        var item = new BreadcrumbTrailItem() { Text = text, IconName = iconName, Uri = uri };
        base.Add(item);
        return item;
    }

    /// <summary>
    /// Add a new menu item into the items collection
    /// </summary>
    /// <param name="item">BreadcrumbTrailItem object</param>
    /// <returns></returns>
    public new BreadcrumbTrailItem Add(BreadcrumbTrailItem item)
    {
        base.Add(item);
        return item;
    }

    /// <summary>
    /// Add a new menu item into the items collection
    /// </summary>
    /// <param name="onClickHandler">Menu ation callback</param>
    /// <param name="text">Menu label</param>
    /// <param name="icon">Menu icon</param>
    /// <returns></returns>
    public BreadcrumbTrailItem AddAction(Action onClickHandler, string text, string icon = null)
    {
        var item = new BreadcrumbTrailItem() { Text = text, IconName = icon, OnClick = onClickHandler };
        base.Add(item);
        return item;
    }

    #endregion

}
