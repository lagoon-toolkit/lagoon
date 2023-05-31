namespace Lagoon.UI.GridView.Components.Internal;

/// <summary>
/// Show the filter summaries
/// </summary>
public partial class LgGridSummaryFilter : LgComponentBase
{

    #region constants


    /// <summary>
    /// Number of summary filters displayed
    /// </summary>
    internal const int NUMBER_SUMMARY_FILTERS_DISPLAYED = 2;

    #endregion

    #region fields

    List<GridViewSummaryFilter> _summaryFilters;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets gridview owner
    /// </summary>
    [Parameter]
    public LgBaseGridView GridView { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadFiltersSummary();
        GridView.OnFilterSummaryStateChange += OnFilterSummaryStateChange;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        GridView.OnFilterSummaryStateChange -= OnFilterSummaryStateChange;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Called when the filter changed.
    /// </summary>
    private void OnFilterSummaryStateChange()
    {
        LoadFiltersSummary();
        StateHasChanged();
    }

    /// <summary>
    /// Load the filter summaries.
    /// </summary>
    private void LoadFiltersSummary()
    {
        _summaryFilters = GridView.ColumnList?.Where(c => c.Filter is not null)
            .Select(c => c.GetSummaryFilter()).ToList();
    }

    /// <summary>
    /// Get hidden filters summary
    /// </summary>
    private string GetHiddenFiltersSummary()
    {
        StringBuilder filtersSummary = new();
        foreach (GridViewSummaryFilter summaryFilter in _summaryFilters.Skip(NUMBER_SUMMARY_FILTERS_DISPLAYED))
        {
            if (filtersSummary.Length != 0)
            {
                filtersSummary.Append("\n\n");
            }
            filtersSummary.Append(summaryFilter.Name);
            filtersSummary.Append(" : ");
            filtersSummary.Append(summaryFilter.Value);
        }
        return filtersSummary.ToString();
    }

    /// <summary>
    /// Get the list of additional attributes. Tooltip
    /// </summary>
    /// <returns>The list of additional attributesof tooltip to add.</returns>
    protected static IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes(string tooltip)
    {
        return GetTooltipAttributes(tooltip, false, TooltipPosition.Bottom);
    }

    #endregion

}
