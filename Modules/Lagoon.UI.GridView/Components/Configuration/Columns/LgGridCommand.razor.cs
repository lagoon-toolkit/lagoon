namespace Lagoon.UI.Components;

/// <summary>
/// Defines a grid commsand.
/// </summary>
public class LgGridCommand : ComponentBase
{
    #region parameters

    /// <summary>
    /// Gets command name
    /// </summary>
    /// <value></value>
    [Parameter]
    public string CommandName { get; set; }

    /// <summary>
    /// Gets text
    /// </summary>
    /// <value></value>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets icon
    /// </summary>
    /// <value></value>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets command aria label
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets command tooltip
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets command column parent
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    private List<LgGridCommand> CommandList { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        CommandList?.Add(this);
    }

    #endregion
}
