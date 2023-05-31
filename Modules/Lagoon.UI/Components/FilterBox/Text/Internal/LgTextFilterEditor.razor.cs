namespace Lagoon.UI.Components.Internal;

/// <summary>
/// A text filter editor.
/// </summary>
public partial class LgTextFilterEditor : LgFilterEditorBase<string, TextFilter>
{

    #region constants

    private const int PREVIEW_COUNT = 8;

    #endregion

    #region fields

    /// <summary>
    /// Filter value of advanced search
    /// </summary>
    private string _filterText;

    /// <summary>
    /// Gets list of additionnal attributes for filter input
    /// </summary>
    private Dictionary<string, object> _filterTextAdditionalAttributes;


    /// <summary>
    /// Filter text operator for advanced search
    /// </summary>
    private FilterTextSearchMode _filterOperator;

    /// <summary>
    /// List of selected string
    /// </summary>
    private HashSet<string> _selectedValues;

    /// <summary>
    /// The selectable items components
    /// </summary>
    private LgSearchableList<string> _searchableList;

    /// <summary>
    /// The preview filtered values
    /// </summary>
    private List<string> _preview;

    #endregion

    #region methods                

    ///<inheritdoc/>
    protected override void LoadFilterParameter(TextFilter filter)
    {
        SetOperator(FilterTextSearchMode.Contains);
        if (filter is not null && !filter.IsEmpty)
        {
            TextFilterItem firstFilterItem = filter.Values.FirstOrDefault();
            if (firstFilterItem is not null)
            {
                IEnumerable<string> valuesList = filter.EnumerateIncludedValues();
                _selectedValues = new HashSet<string>(valuesList);
                if (_selectedValues.Any())
                {
                    SelectedTab = FilterTab.Selection;
                    _filterText = null;
                    _filterOperator = FilterTextSearchMode.Contains;
                }
                else if (!string.IsNullOrEmpty(firstFilterItem.SearchedText))
                {
                    SelectedTab = FilterTab.Rules;
                    _filterText = firstFilterItem.SearchedText;
                    _filterOperator = firstFilterItem.SearchMode;
                }
            }
        }
        else
        {
            _selectedValues = new();
            _filterText = null;
            _filterOperator = FilterTextSearchMode.Contains;
        }
    }

    ///<inheritdoc/>
    protected override bool WorkingItemsRequired()
    {
        // The working items are used in both tabs
        return true;
    }

    ///<inheritdoc/>
    protected override async Task<IEnumerable<string>> GetWorkingItemsAsync(CancellationToken cancellationToken)
    {
        return (await base.GetWorkingItemsAsync(cancellationToken))?.Distinct().Union(_selectedValues).OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
    }

    ///<inheritdoc/>
    protected override TextFilter BuildFilter()
    {
        // Build filter expression            
        if (SelectedTab == FilterTab.Selection)
        {
            List<string> selection = _searchableList.GetSelection().ToList();
            if(selection.Count == 0 || selection.Count == WorkingItems.Count)
            {
                return null;
            }
            return new(selection.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));
        }
        else if (SelectedTab == FilterTab.Rules && !string.IsNullOrEmpty(_filterText))
        {
            return new(_filterOperator, _filterText, App.BehaviorConfiguration.TextFilterCollation.Value);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Define selected search operator
    /// </summary>
    /// <param name="filterOperator"></param>
    private void SetOperator(FilterTextSearchMode filterOperator)
    {
        _filterOperator = filterOperator;
        if (filterOperator == FilterTextSearchMode.StartsWith && FilterBox is LgTextFilterBox fb)
        {
            _filterTextAdditionalAttributes = fb.InputMaskOptions?.GetAttributes();
        }
        else
        {
            _filterTextAdditionalAttributes = null;
        }
        UpdatePreview();
    }

    /// <summary>
    /// Show the preview values when user is typing.
    /// </summary>
    /// <param name="args">Current text.</param>
    private void OnInputFilterText(ChangeEventArgs args)
    {
        _filterText = args.Value?.ToString();
        UpdatePreview();
    }

    /// <summary>
    /// Update the preview.
    /// </summary>
    private void UpdatePreview()
    {
        _preview = null;
        if (!string.IsNullOrEmpty(_filterText) && WorkingItems is not null)
        {
            TextFilter filter = new(_filterOperator, _filterText, App.BehaviorConfiguration.TextFilterCollation.Value);
            _preview = WorkingItems.ApplyFilter(filter, default).Take(PREVIEW_COUNT + 1).ToList();
        }

    }

    #endregion

}
