namespace Lagoon.UI.Components.Internal;

/// <summary>
/// A enum filter editor.
/// </summary>
/// <typeparam name="TValue">The Enum type.</typeparam>
public partial class LgEnumFilterEditor<TValue> : LgFilterEditorBase<TValue, EnumFilter<TValue>>
{

    #region fields

    /// <summary>
    /// List of selected enum
    /// </summary>
    private HashSet<TValue> _selectedValues;

    /// <summary>
    /// The selectable items components
    /// </summary>
    private LgSearchableList<TValue> _searchableList;

    #endregion

    #region methods


    ///<inheritdoc/>
    protected override void LoadFilterParameter(EnumFilter<TValue> filter)
    {
        if (filter is not null && !filter.IsEmpty)
        {
            EnumFilterItem<TValue> valuesList = filter.Values.First();
            _selectedValues = new HashSet<TValue>(valuesList.Values);
        }
        else
        {
            _selectedValues = new();
        }
    }

    ///<inheritdoc/>
    protected override async Task<IEnumerable<TValue>> GetWorkingItemsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<TValue> items = await base.GetWorkingItemsAsync(cancellationToken);
        if (items is null)
        {
            // Load all enumeration values
            List<TValue> enumValues = new();
            Type enumType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            foreach (object value in Enum.GetValues(enumType))
            {
                enumValues.Add((TValue)value);
            }
            items = enumValues;
        }
        else
        {
            // Keep only distinct values of the source
            items = items.Distinct().Union(_selectedValues);
        }
        return items.OrderBy(e => FilterBox.FormatValue(e), StringComparer.OrdinalIgnoreCase);
    }

    ///<inheritdoc/>
    protected override EnumFilter<TValue> BuildFilter()
    {
        List<TValue> selection = _searchableList.GetSelection().ToList();
        if (selection.Count > 0 && selection.Count < WorkingItems.Count)
        {
            return new(selection);
        }
        return null;
    }

    #endregion

}
