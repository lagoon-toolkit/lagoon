namespace Lagoon.UI.Components;

/// <summary>
/// Accordion frames component.
/// </summary>
public partial class LgAccordion : LgComponentBase
{
    #region Properties
    /// <summary>
    /// Gets or sets active pane
    /// </summary>
    private LgAccordionPane ActiveItem { get; set; }

    /// <summary>
    /// Gets or sets accordion content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets css class of the container
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets list of panes
    /// </summary>
    public List<LgAccordionPane> Items { get; set; } = new List<LgAccordionPane>();
    #endregion

    #region methods

    /// <summary>
    /// Add new items inside Accordion
    /// </summary>
    /// <param name="item"></param>
    internal void AddItem(LgAccordionPane item)
    {
        Items.Add(item);
    }

    /// <summary>
    /// Attached event on the open/close bouton click
    /// </summary>
    /// <param name="activeItem"></param>
    internal void HandleClick(LgAccordionPane activeItem)
    {
        ActiveItem = activeItem;
        foreach (var item in Items)
        {
            if (item != ActiveItem)
            {
                item.SetCollapse(true);
            }
        }
        StateHasChanged();
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("accordion", CssClass);
    }

    #endregion
}
