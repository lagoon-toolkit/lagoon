namespace Lagoon.UI.Components;

/// <summary>
/// A Global Search item list.
/// </summary>
public class LgGlobalSearchItemCollection : List<LgPageLink>
{

    /// <summary>
    /// Add new item to the collection.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="title"></param>
    public void Add(string uri, string title)
    {
        Add(new LgGlobalSearchItem(uri, title));
    }

}
