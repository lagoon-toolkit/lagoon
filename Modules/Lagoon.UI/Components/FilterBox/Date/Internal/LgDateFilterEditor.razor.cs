namespace Lagoon.UI.Components.Internal;

/// <summary>
/// A date filter editor component.
/// </summary>
/// <typeparam name="TDateTime">The type of value.</typeparam>
public partial class LgDateFilterEditor<TDateTime> : LgFilterEditorBase<TDateTime, DateFilter<TDateTime>>
{

    #region fields

    /// <summary>
    /// Datasource for Treeview
    /// </summary>
    private TreeNodeCollection<FilterDatePeriod> _rootNodes;

    /// <summary>
    /// Indicate if all items are selected
    /// </summary>
    private bool _allSelected;

    /// <summary>
    /// Value to find
    /// </summary>
    private string _searchValue;

    /// <summary>
    /// Search cancellation token
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource;

    #endregion

    #region parameters        

    /// <summary>
    /// Gets or sets first date
    /// </summary>
    [Parameter]
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets last date
    /// </summary>        
    [Parameter]
    public DateTime? To { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _cancellationTokenSource?.Dispose();
        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    protected override void LoadFilterParameter(DateFilter<TDateTime> filter)
    {
        FilterRange<TDateTime> range = filter?.GetFirstRange();
        if (range is not null)
        {
            SelectedTab = FilterTab.Rules;
            if (range.Minimum is not null)
            {
                From = ToNullableDateTime(range.Minimum.Value);
            }
            if (range.Maximum is not null)
            {
                To = ToNullableDateTime(range.Maximum.Value);
            }
        }
        else
        {
            FilterDatePeriodCollection periods = filter?.GetFirstPeriodCollection();
            if (periods is not null)
            {
                SelectedTab = FilterTab.Selection;
            }
        }
    }

    /// <summary>
    /// Convert a TDateTime to DateTime?.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The date.</returns>
    private static DateTime? ToNullableDateTime(TDateTime date)
    {
        if (date is DateTime dt)
        {
            return dt;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Convert a TDateTime to DateTime?.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The date.</returns>
    private static TDateTime ToTDateTime(DateTime date)
    {
        // Convert DateTime value to the target generic type
        if (typeof(TDateTime) == typeof(DateTime?))
        {
            DateTime? dt = date;
            return System.Runtime.CompilerServices.Unsafe.As<DateTime?, TDateTime>(ref dt);
        }
        else
        {
            return System.Runtime.CompilerServices.Unsafe.As<DateTime, TDateTime>(ref date);
        }
    }

    ///<inheritdoc/>
    protected override DateFilter<TDateTime> BuildFilter()
    {
        DateFilter<TDateTime> filter = new();
        if (SelectedTab == FilterTab.Rules)
        {
            FilterRange<TDateTime> range = new();
            if (From is not null)
            {
                range.Minimum = new(ToTDateTime(From.Value.Date));
            }
            if (To is not null)
            {
                range.Maximum = new(ToTDateTime(To.Value.Date));
            }
            if (!range.IsEmpty)
            {
                filter.AddBetween(range);
            }
        }
        else
        {
            filter.AddIncludedInPeriods(_rootNodes.EnumerateSelected().Select(n => n.Item));
        }
        return filter;
    }

    ///<inheritdoc/>
    protected override async Task<IEnumerable<TDateTime>> GetWorkingItemsAsync(CancellationToken cancellationToken)
    {
        // Starts retrieving items
        Task<IEnumerable<TDateTime>> task = base.GetWorkingItemsAsync(cancellationToken);
        // Build the current filter value list
        FilterDatePeriodCollection periods = FilterBox.Filter?.GetFirstPeriodCollection();
        List<TDateTime> selectedValues = new();
        List<DateTime> listSelectedValue = selectedValues as List<DateTime>;
        List<DateTime?> nullableListSelectedValue = selectedValues as List<DateTime?>;
        if (periods is not null)
        {
            if (listSelectedValue is not null)
            {
                foreach (DateTime? dt in GetSelectedDates(periods))
                {
                    if (dt.HasValue)
                    {
                        listSelectedValue.Add(dt.Value);
                    }
                }
            }
            else
            {
                nullableListSelectedValue.AddRange(GetSelectedDates(periods));
            }
        }
        // Get all the unique dates sorted
        IEnumerable<TDateTime> workingitems = await task;
        cancellationToken.ThrowIfCancellationRequested();
        workingitems ??= new List<TDateTime>();
        if (workingitems is IEnumerable<DateTime> list)
        {
            workingitems = (IEnumerable<TDateTime>)list.Select(d => d.Date).Distinct()
                .Union(listSelectedValue, new UnionComparer()).OrderBy(d => d).ToList();
        }
        else if (workingitems is IEnumerable<DateTime?> nullableList)
        {
            workingitems = (IEnumerable<TDateTime>)nullableList.Select(d => d?.Date).Distinct()
                .Union(nullableListSelectedValue, new NullableUnionComparer()).OrderBy(d => d).ToList();
        }
        // Build the list
        cancellationToken.ThrowIfCancellationRequested();
        // Load all visible node
        _rootNodes = GetTreeViewNodes(workingitems);
        cancellationToken.ThrowIfCancellationRequested();
        // Select periods
        InitializeTreeViewSelection(periods);
        // Return the item list
        return workingitems;
    }

    private class UnionComparer : IEqualityComparer<DateTime>
    {
        bool IEqualityComparer<DateTime>.Equals(DateTime x, DateTime y)
        {
            return x.Date == y.Date;
        }

        int IEqualityComparer<DateTime>.GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }

    private class NullableUnionComparer : IEqualityComparer<DateTime?>
    {
        bool IEqualityComparer<DateTime?>.Equals(DateTime? x, DateTime? y)
        {
            return x?.Date == y?.Date;
        }

        int IEqualityComparer<DateTime?>.GetHashCode(DateTime? obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Get the current filter selected date.
    /// </summary>
    /// <param name="periods">The list of period for the filter.</param>
    /// <returns>The list of dates.</returns>
    private static IEnumerable<DateTime?> GetSelectedDates(FilterDatePeriodCollection periods)
    {
        foreach (FilterDatePeriod period in periods)
        {
            if (period is null)
            {
                yield return null;
            }
            else
            {
                for (int i = 0; i < period.Duration; i++)
                {

                    yield return AddDuration(period, i);
                }
            }
        }
    }

    /// <summary>
    /// Return a new date with the added duration by using the kind of the period.
    /// </summary>
    /// <param name="period">The period.</param>
    /// <param name="duration">The duration to add.</param>
    /// <returns>The new date with the added duration.</returns>
    private static DateTime AddDuration(FilterDatePeriod period, int duration)
    {
        return period.Kind switch
        {
            FilterDatePeriodKind.Year => period.IncludedStart.AddYears(duration).AddHours(2), // H+2 = Year only date
            FilterDatePeriodKind.Month => period.IncludedStart.AddMonths(duration).AddHours(1), // H+1 = Month only date
            FilterDatePeriodKind.Day => period.IncludedStart.AddDays(duration),
            _ => throw new ArgumentOutOfRangeException(nameof(period))
        };
    }

    /// <summary>
    /// Initialise the treeview selection.
    /// </summary>
    private void InitializeTreeViewSelection(FilterDatePeriodCollection periods)
    {
        if (periods is not null)
        {
            foreach (TreeNode<FilterDatePeriod> node in _rootNodes.EnumerateHierarchy())
            {
                if (periods.ContainsForKind(node.Item))
                {
                    SelectNode(node, true);
                    TreeNode<FilterDatePeriod> parent = node.Parent;
                    while (parent is not null)
                    {
                        parent.Expanded = true;
                        parent = parent.Parent;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Build treeview data source
    /// </summary>
    /// <returns></returns>
    private static TreeNodeCollection<FilterDatePeriod> GetTreeViewNodes(IEnumerable<TDateTime> workingitems)
    {
        TreeNodeCollection<FilterDatePeriod> treeviewNodes = new();
        int prevMonth = 0;
        int prevYear = 0;
        TreeNode<FilterDatePeriod> yearNode = null;
        TreeNode<FilterDatePeriod> monthNode = null;

        foreach (TDateTime item in workingitems)
        {
            if (item is DateTime date)
            {
                if (prevYear != date.Year)
                {
                    prevYear = date.Year;
                    prevMonth = 0;
                    // Remark : The .Date is for the year that came from current filter value.
                    yearNode = treeviewNodes.Add(new FilterDatePeriod(FilterDatePeriodKind.Year, date.Date));
                }
                if (prevMonth != date.Month)
                {
                    prevMonth = date.Month;
                    // We don't add the year only date
                    if (date.Hour <= 1)
                    {
                        // Remark : The .Date is for the year that came from current filter value.
                        monthNode = yearNode.AddChild(new FilterDatePeriod(FilterDatePeriodKind.Month, date.Date));
                    }
                }
                // We don't add the year only or month only date
                if (date.Hour == 0)
                {
                    monthNode.AddChild(new FilterDatePeriod(FilterDatePeriodKind.Day, date));
                }
            }
            else
            {
                //Null value
                treeviewNodes.Add((FilterDatePeriod)null);
            }
        }
        return treeviewNodes;
    }

    private void SelectPeriod(ChangingEventArgs<bool?> args)
    {
        try
        {
            TreeNode<FilterDatePeriod> selectedNode = (TreeNode<FilterDatePeriod>)args.Item;
            // Change the value from indeterminate state for days
            if (args.OldValue is null)
            {
                args.NewValue = false;
            }
            SelectNode(selectedNode, args.NewValue.Value);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private void SelectNode(TreeNode<FilterDatePeriod> node, bool selected)
    {
        node.CheckNode(selected);
        // Update Select All indicator
        _allSelected = _rootNodes.All(n => n.IsChecked);
    }

    /// <summary>
    /// Select or unselect all items
    /// </summary>
    /// <param name="args"></param>
    private void ToggleSelectAll(ChangeEventArgs args)
    {
        bool selectAll = (bool)args.Value;
        _rootNodes.ForEach(n => { if (!n.Hidden) { SelectNode(n, selectAll); } });
    }

    /// <summary>
    /// Raise the search.
    /// </summary>
    /// <param name="args">The searched string.</param>
    private async Task OnSearchUpdateAsync(ChangeEventArgs args)
    {
        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();
        _searchValue = args.Value?.ToString();
        await Task.Delay(500, _cancellationTokenSource.Token).ContinueWith(tr =>
        {
            if (!tr.IsCanceled)
            {
                HideFiltredNodes();
            }
        });
    }

    /// <summary>
    /// Hide the node that don't contains the searched text.
    /// </summary>
    private void HideFiltredNodes()
    {
        if (string.IsNullOrEmpty(_searchValue))
        {
            _rootNodes.ForEachInHierarchy(n => n.Hidden = false);
        }
        else
        {
            TreeNode<FilterDatePeriod> yearNode = null;
            TreeNode<FilterDatePeriod> monthNode = null;
            foreach (TreeNode<FilterDatePeriod> node in _rootNodes.EnumerateHierarchy())
            {
                if (node.Item is null)
                {
                    node.Hidden = true;
                }
                else
                {
                    node.Hidden = !node.Item.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase);
                    if (node.Item.Kind == FilterDatePeriodKind.Year)
                    {
                        yearNode = node;
                    }
                    else
                    {
                        if (!node.Hidden)
                        {
                            yearNode.Hidden = false;
                        }
                        if (node.Item.Kind == FilterDatePeriodKind.Month)
                        {
                            monthNode = node;
                        }
                        else
                        {
                            if (!node.Hidden)
                            {
                                monthNode.Hidden = false;
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

}
