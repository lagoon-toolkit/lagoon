namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component to handle the items load from render fragment.
/// </summary>
/// <typeparam name="TValue">Select "Value" type.</typeparam>
/// <typeparam name="TItemValue">Item "Value" type.</typeparam>
public sealed partial class LgSelectItemsLoader<TValue, TItemValue> : ComponentBase, IDisposable
{

    #region parameters

    /// <summary>
    /// Gets the parent LgSelectBase component.
    /// </summary>
    [Parameter]
    public LgSelectBase<TValue, TItemValue> Select { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Select.OnReloadItemsContent += OnReloadItemsContent;
    }

    /// <summary>
    /// Unregister event listener.
    /// </summary>
    public void Dispose()
    {
        Select.OnReloadItemsContent -= OnReloadItemsContent;
    }

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        return Select.ShowList || Select.HasUnknownValues;
    }

    ///<inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        // Show the selected item(s) with they're Text property in the button of the select component
        if (Select.HasUnknownValues)
        {
            Select.HasUnknownValues = false;
            Select.UpdateSelection();
        }
        // Try to refresh the "Select All" state
        if (Select.ShowList)
        {
            Select.SelectAllManager?.UpdateRender();
        }
    }

    /// <summary>
    /// Redo the Items.
    /// </summary>
    private void OnReloadItemsContent()
    {
        StateHasChanged();
    }

    #endregion

}
