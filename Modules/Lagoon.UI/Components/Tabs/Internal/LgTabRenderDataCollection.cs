using System.Collections;

namespace Lagoon.UI.Components.Internal;

internal class LgTabRenderDataCollection : IEnumerable<LgTabRenderData>
{

    #region fields

    /// <summary>
    /// Gets or sets tabs order list
    /// </summary>
    private List<string> TabsOrder { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets tabs list
    /// </summary>
    private Dictionary<string, LgTabRenderData> Tabs { get; set; } = new Dictionary<string, LgTabRenderData>();

    #endregion

    #region properties

    public LgTabRenderData this[string key] => Tabs[key];

    public LgTabRenderData this[int index] => Tabs[TabsOrder[index]];

    public int Count => Tabs.Count;

    #endregion

    #region interfaces

    public IEnumerator<LgTabRenderData> GetEnumerator()
    {
        return GetTabs().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetTabs().GetEnumerator();
    }

    #endregion

    #region methods

    /// <summary>
    /// Determines whether the specified key is already registred.
    /// </summary>
    /// <param name="key">Tab key</param>
    /// <returns>true if the specified key is already registred.</returns>
    internal bool ContainsKey(string key)
    {
        return Tabs.ContainsKey(key);
    }

    /// <summary>
    /// Return a list key of all opened tabs
    /// </summary>
    public IEnumerable<string> GetKeys()
    {
        return Tabs.Keys;
    }

    /// <summary>
    /// Return a ordered list of tabs.
    /// </summary>
    /// <returns>A ordered list of tabs.</returns>
    public IEnumerable<LgTabRenderData> GetTabs()
    {
        foreach (string key in TabsOrder)
        {
            yield return Tabs[key];
        }
    }

    /// <summary>
    /// Try to retrieve the render data for tab key.
    /// </summary>
    /// <param name="key">Tab key</param>
    /// <param name="tabRenderData">Tab render data.    </param>
    /// <returns></returns>
    public bool TryGet(string key, out LgTabRenderData tabRenderData)
    {
        return Tabs.TryGetValue(key, out tabRenderData);
    }

    /// <summary>
    /// Add new tab in container
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tab"></param>
    public void Add(string key, LgTabRenderData tab)
    {
        Tabs.Add(key, tab);
        TabsOrder.Add(key);
    }

    /// <summary>
    /// Remove tab by its id
    /// </summary>
    /// <param name="key">Tab id</param>
    public void Remove(string key)
    {
        Tabs.Remove(key);
        TabsOrder.Remove(key);
    }

    public int IndexOf(string key)
    {
        return TabsOrder.IndexOf(key);
    }

    public void Move(string key, int index)
    {
        TabsOrder.Remove(key);
        TabsOrder.Insert(index, key);
    }

    /// <summary>
    /// Change the spin state of an tab
    /// </summary>
    public void PinTab(string key)
    {
        // Move tab in the first position
        LgTabRenderData tab = Tabs[key];
        int indexRecepted = -1;
        if (!tab.Pinned)
        {
            foreach (LgTabRenderData checkedTab in GetTabs())
            {
                if (checkedTab.Draggable && !checkedTab.Pinned)
                {
                    indexRecepted = TabsOrder.IndexOf(checkedTab.Key);
                    break;
                }
            }
        }
        else
        {
            indexRecepted = TabsOrder.Count - 1;
        }
        if (indexRecepted > -1)
        {
            TabsOrder.Remove(tab.Key);
            TabsOrder.Insert(indexRecepted, tab.Key);
        }
        tab.TabData.Pinned = !tab.Pinned;
    }

    #endregion

}
