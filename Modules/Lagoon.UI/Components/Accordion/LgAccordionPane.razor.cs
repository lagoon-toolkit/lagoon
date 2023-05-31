namespace Lagoon.UI.Components;

/// <summary>
/// Accordion pane.
/// </summary>
public partial class LgAccordionPane : LgFrame
{
    #region Properties

    /// <summary>
    /// Gets or sets accordion pane container
    /// </summary>
    [CascadingParameter]
    public LgAccordion Container { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Initialize new instance.
    /// </summary>
    public LgAccordionPane() : base()
    {
        Collapsable = true;
        DefaultCollapsed = true;
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        CssClass = "accordion-item";
        if (Container != null)
        {
            Container.AddItem(this);
            OnToggle = EventCallback.Factory.Create<ToggleFrameEventArgs>(this, (ToggleFrameEventArgs args) =>
            {
                Container.HandleClick(this);
            });
        }
    }

    /// <summary>
    /// Define if the state is opened or closed
    /// </summary>
    /// <param name="state"></param>
    internal void SetCollapse(bool state)
    {
        Collapsed = state;
        SetIconState();
    }
    #endregion
}
