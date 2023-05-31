namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Filter box to filter bool or bool? values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgBooleanFilterEditor<TValue> : LgFilterEditorBase<TValue, BooleanFilter<TValue>>
{

    #region fields

    /// <summary>
    /// Selected values
    /// </summary>
    private HashSet<TValue> _selectedValues;

    /// <summary>
    /// The selectable items components
    /// </summary>
    private LgSearchableList<TValue> _searchableList;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void LoadFilterParameter(BooleanFilter<TValue> filter)
    {
        if (filter is not null && !filter.IsEmpty)
        {
            BooleanFilterItem<TValue> valuesList = filter.Values.First();
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
            List<TValue> boolValues = new();
            if (boolValues is List<bool> list)
            {
                list.Add(false);
                list.Add(true);
            }
            else if (boolValues is List<bool?> nullableList)
            {
                nullableList.Add(null);
                nullableList.Add(false);
                nullableList.Add(true);
            }
            return boolValues;
        }
        else
        {
            return items.Distinct().Union(_selectedValues).OrderBy(v => v);
        }
    }

    ///<inheritdoc/>
    protected override BooleanFilter<TValue> BuildFilter()
    {
        List<TValue> selection = _searchableList.GetSelection().ToList();
        if (selection.Count > 0 && selection.Count < WorkingItems.Count)
        {
            return new(selection);
        }
        else
        {
            return null;
        }
    }

    #endregion

}
