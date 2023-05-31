namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component to select all items in the drop down list.
/// </summary>
/// <typeparam name="TValue">The type for list of values.</typeparam>
/// <typeparam name="TItemValue">The type of values.</typeparam>
public partial class LgSelectAll<TValue, TItemValue> : LgComponentBase, IDisposable
{
    #region cascading parameters

    /// <summary>
    /// Guid to force OnParameterSet and update the selection.
    /// </summary>
    [CascadingParameter]
    public Guid SelectionGuid { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets the parent LgSelectBase component.
    /// </summary>
    [Parameter]
    public LgSelectBase<TValue, TItemValue> Select { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the visible items in the list.
    /// </summary>
    private IEnumerable<LgOptionListItem<TItemValue>> VisibleItems
        => Select.OptionItems.Values.Select(i => (LgOptionListItem<TItemValue>) i)
            .Where(d => Select.IsDropdownItemVisible(d));

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Select.SelectAllManager.OnUpdated += OnSelectionUpdated;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        Select.SelectAllManager.OnUpdated -= OnSelectionUpdated;
        base.Dispose(disposing);
    }

    private void OnSelectionUpdated()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Get the current selection state.
    /// </summary>
    /// <returns></returns>
    private bool? GetSelectAllState()
    {
        int selected = 0;
        int unselected = 0;
        foreach (var optionItem in VisibleItems)
        {
            if (optionItem.IsSelected)
                selected++;
            else
                unselected++;
        }
        if (selected == 0)
        {
            return false;
        }
        else
        {
            return unselected == 0 ? true : null;
        }
    }

    /// <summary>
    /// Determine the new state of the items available in dropdown.
    /// Checkbox SelectAll is false => unselect all items
    /// Checkbox SelectAll is true => select all items
    /// If Checkbox SelectAll is null => select all items that are not already selected and set checkbox to checked
    /// </summary>
    /// <returns></returns>
    private async Task ToggleSelectAllAsync(ChangeEventArgs args)
    {
        bool? value = (bool?)args.Value;
        if(Select is LgSelectMultiple<TItemValue> select)
        {
            if (value.HasValue && !value.Value)
            {
                // Unselect All
                await Select.ChangeSelectionAsync(VisibleItems.Where(x => x.IsSelected), false);
            }
            else
            {
                // Select All
                await Select.ChangeSelectionAsync(VisibleItems.Where(x => !x.IsSelected), true);
            }
        }
        Select.UpdateSelection();
    }

    #endregion

}
