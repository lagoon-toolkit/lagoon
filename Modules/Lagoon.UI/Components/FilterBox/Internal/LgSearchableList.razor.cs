using Lagoon.UI.Application;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Checkbox list with search input
/// </summary>
public partial class LgSearchableList<TValue> : ComponentBase
{

    #region fields

    /// <summary>
    /// Indicate if the current selection must be added to the existing selection.
    /// </summary>
    private bool _addToSelection;

    /// <summary>
    /// Indicate if all visible items are selected.
    /// </summary>
    private bool? _allSelected;

    /// <summary>
    /// The compare options.
    /// </summary>
    private CompareOptions _compareOptions;

    /// <summary>
    /// The search filter.
    /// </summary>
    private string _searchValue;

    /// <summary>
    /// The list of selected items.
    /// </summary>
    private HashSet<TValue> _selection;

    /// <summary>
    /// Indicate if the "Add to selection" button is visible.
    /// </summary>
    private bool _showAddToSelection;

    /// <summary>
    /// The list of visible items.
    /// </summary>
    private List<TValue> _visibleItems;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets List of the objects
    /// </summary>
    [Parameter]
    public List<TValue> Items { get; set; }

    /// <summary>
    /// Gets or set list of selected objects
    /// </summary>
    [Parameter]
    public ICollection<TValue> SelectedItems { get; set; }

    /// <summary>
    /// Gets or sets the method to use to format value.
    /// </summary>
    [Parameter]
    public Func<TValue, string> FormatValue { get; set; }

    /// <summary>
    /// Gets or sets the method to use to format value in search.
    /// </summary>
    [Parameter]
    public Func<TValue, string> FormatSearchValue { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Give access to LgApplication context
    /// </summary>
    [Inject]
    public LgApplication App { get; set; }

    /// <summary>
    /// Gests if the list is filtred.
    /// </summary>
    public bool IsFiltred => !string.IsNullOrEmpty(_searchValue);

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _compareOptions = App.BehaviorConfiguration.TextFilterCollation.Value.ToCompareOptions();
        FormatValue ??= v => v?.ToString();
        FormatSearchValue ??= FormatValue;
        _selection = new(SelectedItems);
        _visibleItems = Items;
        UpdateAllSelectedState();
    }

    /// <summary>
    /// Raise the search.
    /// </summary>
    /// <param name="args"></param>
    private void OnSearchTextInput(ChangeEventArgs args)
    {
        _searchValue = args.Value?.ToString();
        _visibleItems = LoadVisibleItems();
        _selection.Clear();
        if (IsFiltred)
        {
            _selection = new(_visibleItems);
            _showAddToSelection = SelectedItems.Count > 0;
        }
        else
        {
            _selection = new(SelectedItems);
            _showAddToSelection = false;
        }
        UpdateAllSelectedState();
    }

    /// <summary>
    /// An item is selected.
    /// </summary>
    private void OnSelection()
    {
        UpdateAllSelectedState();
    }

    /// <summary>
    /// Update the all selected state.
    /// </summary>
    private void UpdateAllSelectedState()
    {
        int count = _visibleItems.Where(v => _selection.Contains(v)).Count();
        if (count == 0)
        {
            _allSelected = false;
        }
        else if (count == _visibleItems.Count)
        {
            _allSelected = true;
        }
        else
        {
            _allSelected = null;
        }
    }

    /// <summary>
    /// Get the list of values to select.
    /// </summary>
    /// <returns></returns>
    private List<TValue> LoadVisibleItems()
    {
        if (string.IsNullOrEmpty(_searchValue))
        {
            return Items;
        }
        else
        {
            return Items.Where(v => Contains(v)).ToList();
        }
    }

    private bool Contains(TValue value)
    {
        string svalue = FormatSearchValue(value);
        return svalue is not null && CultureInfo.InvariantCulture.CompareInfo.IndexOf(svalue, _searchValue, _compareOptions) != -1;
    }

    /// <summary>
    /// Select or unselect all items
    /// </summary>
    /// <param name="args"></param>
    private void ToggleSelectAll(ChangeEventArgs args)
    {
        _allSelected = (bool)args.Value;
        if (_allSelected.Value)
        {
            foreach (TValue item in _visibleItems)
            {
                if (!_selection.Contains(item))
                {
                    _selection.Add(item);
                }
            }
        }
        else
        {
            if (_visibleItems == Items)
            {
                _selection.Clear();
            }
            else
            {
                foreach (TValue item in _visibleItems)
                {
                    _selection.Remove(item);
                }
            }
        }
    }

    /// <summary>
    /// Get the selected items.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TValue> GetSelection()
    {
        if (_showAddToSelection && _addToSelection)
        {
            HashSet<TValue> selection = new(SelectedItems);
            foreach (TValue item in _visibleItems.Except(_selection))
            {
                selection.Remove(item);
            }
            foreach (TValue item in _selection)
            {
                if (!selection.Contains(item))
                {
                    selection.Add(item);
                }
            }
            return selection;
        }
        else
        {
            return _selection;
        }
    }

    #endregion

}
