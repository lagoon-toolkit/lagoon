namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Create a data source from an Enum declaration. Use the [Display("#sample")] attribute on enumeration values
/// to define the text.
/// </summary>
/// <typeparam name="TItem">The enum type to use.</typeparam>
/// <typeparam name="TValue">The enum potentially nullable.</typeparam>
public abstract class EnumListDataSourceBase<TItem, TValue> : ListDataSourceBase<TItem, TValue> where TItem : Enum
{

    #region private classes

    private class EnumListItemData : ListItemData<TValue>
    {
        /// <summary>
        /// The order weight of the column.
        /// </summary>
        public int Order { get; set; }

        public EnumListItemData(TItem item, TValue value)
        {
            Value = value;
            System.ComponentModel.DataAnnotations.DisplayAttribute attribute = item.GetDisplayAttribute();
            string name = attribute?.GetName();
            if (name is null)
            {
                Text = value.ToString();
            }
            else
            {
                Text = name.CheckTranslate();
            }
            Tooltip = attribute?.GetDescription();
            Order = attribute?.GetOrder() ?? 0;
        }

    }

    #endregion

    #region fields

    private readonly Dictionary<TItem, IListItemData<TValue>> _dico = new();
#if ADVANCED_CONSTRUCTOR
    private readonly bool _sorted;
    private readonly TItem[] _ignoredValues;
#endif

#endregion

#region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public EnumListDataSourceBase() : base(false) {
        LoadItems();
    }

#if ADVANCED_CONSTRUCTOR
/* !!!!!!!!!!!! Don't activate this, prefer to promote the use of "DisplayAttribute".Order/Name ... */

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="sorted">If <c>true</c> the list is sorted by text; else the original order is preserved.</param>
    /// <param name="ignoredValues">List of values to ignore.</param>
    public EnumListDataSourceBase(bool sorted = false, params TItem[] ignoredValues) : base(false)
    {
        _sorted = sorted;
        _ignoredValues = ignoredValues;
        LoadItems();
    }

#endif

#endregion

#region methods

    /// <summary>
    /// Load enum display names.
    /// </summary>
    private void LoadItems()
    {
        IEnumerable<TItem> enums = GetEnumValues();
#if ADVANCED_CONSTRUCTOR

        if (_ignoredValues.Length > 0)
        {
            enums = enums.Where(i => !_ignoredValues.Contains(i));
        }
#endif
        IEnumerable<ListItemData<TValue>> list = enums.Select(value => new EnumListItemData(value, GetItemValue(value)))
            .Where(i => i.Text != "") // We don't show [Display(Name="")] values
            .OrderBy(i => i.Order).ThenBy(i => i.Text);
#if ADVANCED_CONSTRUCTOR
        if (_sorted)
        {
            list = list.OrderBy(i => i.Text);
        }
#endif
        foreach (ListItemData<TValue> item in list)
        {
            _dico.Add(GetValueItem(item.Value), item);
        }
    }

    /// <summary>
    /// Return the enum values.
    /// </summary>
    /// <returns>The enum values.</returns>
    protected virtual IEnumerable<TItem> GetEnumValues()
    {
        return (TItem[])Enum.GetValues(typeof(TItem));
    }

    ///<inheritdoc/>
    protected override IEnumerable<IListItemData<TValue>> GetItemDataList()
    {
        return _dico.Values;
    }

    ///<inheritdoc/>
    public override string GetItemText(TItem value)
    {
        if (_dico.TryGetValue(value, out IListItemData<TValue> item))
        {
            return item.GetText();
        }
        else
        {
            return value?.ToString();
        }
    }

    ///<inheritdoc/>
    internal abstract TItem GetValueItem(TValue value);

#endregion

}
