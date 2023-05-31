namespace Lagoon.UI.Components.Internal;

/// <summary>
/// A Numeric filter editor.
/// </summary>
/// <typeparam name="TValue">The type of the field to filter.</typeparam>
/// <typeparam name="TNullableValue">The type of the field to filter if it's nullable or the nullable type of the type.</typeparam>
public partial class LgNumericFilterEditor<TValue, TNullableValue> : LgFilterEditorBase<TValue, NumericFilter<TValue>>
{

    #region fields

    /// <summary>
    /// List of numeric filter values
    /// </summary>
    private List<FilterNumericValue<TValue, TNullableValue>> _filterValues = new();

    /// <summary>
    /// List of selected string
    /// </summary>
    private HashSet<TValue> _selectedValues = new();

    /// <summary>
    /// The selectable items components
    /// </summary>
    private LgSearchableList<TValue> _searchableList;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void LoadFilterParameter(NumericFilter<TValue> filter)
    {
        _filterValues.Clear();
        _selectedValues = new();
        if (filter is not null && !filter.IsEmpty)
        {
            foreach (NumericFilterItem<TValue> filterItem in filter.Values)
            {
                if (filterItem.Range is not null)
                {
                    SelectedTab = FilterTab.Rules;
                    FilterRangeLimit<TValue> lesserLimit = filterItem.Range.Maximum;
                    FilterRangeLimit<TValue> greaterLimit = filterItem.Range.Minimum;
                    if (lesserLimit is not null && greaterLimit is not null)
                    {
                        // Range
                        FilterNumericOperator lesserOperator = greaterLimit.Excluded ?
                                    FilterNumericOperator.GreaterThan : FilterNumericOperator.GreaterThanOrEqual;
                        _filterValues.Add(new(greaterLimit.Value, lesserOperator));
                        if (lesserOperator == FilterNumericOperator.GreaterThan ||
                                lesserOperator == FilterNumericOperator.GreaterThanOrEqual)
                        {
                            FilterNumericOperator greaterOperator = lesserLimit.Excluded ?
                                    FilterNumericOperator.LessThan : FilterNumericOperator.LessThanOrEqual;
                            _filterValues.Add(new(lesserLimit.Value, greaterOperator,
                                new List<FilterNumericOperator>
                                {
                                    FilterNumericOperator.LessThan, FilterNumericOperator.LessThanOrEqual
                                }));
                        }
                    }
                    else if (greaterLimit is not null)
                    {
                        // Greater
                        _filterValues.Add(new(greaterLimit.Value, greaterLimit.Excluded ?
                                    FilterNumericOperator.GreaterThan : FilterNumericOperator.GreaterThanOrEqual
                            ));
                    }
                    else
                    {
                        // Lesser
                        _filterValues.Add(new(lesserLimit.Value, lesserLimit.Excluded ?
                                    FilterNumericOperator.LessThan : FilterNumericOperator.LessThanOrEqual
                            ));
                    }
                }
                else
                {
                    if (filterItem.Exclude)
                    {
                        // Not Equal
                        SelectedTab = FilterTab.Rules;
                        _filterValues.Add(new(filterItem.Values.FirstOrDefault(), FilterNumericOperator.NotEqual));
                    }
                    else
                    {
                        // List
                        _selectedValues = new HashSet<TValue>(filterItem.Values);
                        // Only one value is selected
                        if (_selectedValues.Count == 1)
                        {
                            // Equals                        
                            SelectedTab = FilterTab.Rules;
                            _filterValues.Add(new(filterItem.Values.FirstOrDefault(), FilterNumericOperator.Equal));
                        }
                        else
                        {
                            SelectedTab = FilterTab.Selection;
                            _filterValues.Add(new());
                        }
                    }
                }
            }
        }
        else
        {
            // Default range input
            _filterValues.Add(new());
        }
    }

    ///<inheritdoc/>
    protected override async Task<IEnumerable<TValue>> GetWorkingItemsAsync(CancellationToken cancellationToken)
    {
        return (await base.GetWorkingItemsAsync(cancellationToken))?.Distinct().Union(_selectedValues).OrderBy(x => x);
    }

    ///<inheritdoc/>
    protected override NumericFilter<TValue> BuildFilter()
    {
        NumericFilter<TValue> filter = new();
        if (SelectedTab == FilterTab.Selection)
        {
            List<TValue> selection = _searchableList.GetSelection().ToList();
            if (selection.Count > 0 && selection.Count < WorkingItems.Count)
            {
                filter.AddIncludedInList(selection);
            }
        }
        else if (SelectedTab == FilterTab.Rules)
        {
            FilterRange<TValue> range = new();
            foreach (FilterNumericValue<TValue, TNullableValue> uiFilter in _filterValues.Where(f => f.Value is not null))
            {
                switch (uiFilter.FilterOperator)
                {
                    case FilterNumericOperator.LessThan:
                        range.Maximum = new(uiFilter.GetTValue(), true);
                        break;
                    case FilterNumericOperator.LessThanOrEqual:
                        range.Maximum = new(uiFilter.GetTValue());
                        break;
                    case FilterNumericOperator.GreaterThan:
                        range.Minimum = new(uiFilter.GetTValue(), true);
                        break;
                    case FilterNumericOperator.GreaterThanOrEqual:
                        range.Minimum = new(uiFilter.GetTValue());
                        break;
                    case FilterNumericOperator.NotEqual:
                        filter.AddNotEquals(uiFilter.GetTValue());
                        break;
                    default:
                        filter.AddEquals(uiFilter.GetTValue());
                        break;
                }
            }
            if (!range.IsEmpty)
            {
                filter.AddBetween(range);
            }
        }
        return filter;
    }

    /// <summary>
    /// Define selected search operator for the item
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filterOperator"></param>
    private void SetOperator(FilterNumericValue<TValue, TNullableValue> value, FilterNumericOperator filterOperator)
    {
        value.FilterOperator = filterOperator;
        if (_filterValues[0] == value)
        {
            // Add new filter field with limited operator
            _filterValues.RemoveAll(i => i != value);
            if (filterOperator == FilterNumericOperator.GreaterThan ||
                filterOperator == FilterNumericOperator.GreaterThanOrEqual)
            {
                _filterValues.Add(new FilterNumericValue<TValue, TNullableValue>(new List<FilterNumericOperator>
                {
                    FilterNumericOperator.LessThan, FilterNumericOperator.LessThanOrEqual
                }));
            }
        }
    }

    /// <summary>
    /// Label of the filter numeric operator
    /// </summary>
    /// <param name="filterOperator"></param>
    /// <returns></returns>
    private static string GetOperatorLabel(FilterNumericOperator filterOperator)
    {
        return filterOperator.GetDisplayName();
    }

    /// <summary>
    /// Remove the number group separator from serialized values.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Return the formated value without number group separators.</returns>
    private string FormatSearchValue(TValue value)
    {
        return FilterBox.FormatFilterValue(value).Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "");
    }

    #endregion

}
