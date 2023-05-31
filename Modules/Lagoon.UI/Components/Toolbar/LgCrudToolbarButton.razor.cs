namespace Lagoon.UI.Components;

/// <summary>
/// CRUD Toolbar button
/// </summary>
public partial class LgCrudToolbarButton : LgToolbarButton
{
    #region Parameters

    /// <summary>
    /// Gets or sets the save button Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnSave { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the save action.
    /// </summary>
    [Parameter]
    public string OnSaveConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets the save button tooltip.
    /// </summary>
    [Parameter]
    public string SaveButtonTooltip { get; set; }

    /// <summary>
    /// Gets or sets the save button aria label.
    /// </summary>
    [Parameter]
    public string SaveButtonAria { get; set; }

    /// <summary>
    /// Gets or sets the remove button Onclick eventCallback
    /// </summary>
    [Parameter]
    public EventCallback<ActionEventArgs> OnRemove { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the remove action.
    /// </summary>
    [Parameter]
    public string OnRemoveConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets the remove button tooltip.
    /// </summary>
    [Parameter]
    public string RemoveButtonTooltip { get; set; }

    /// <summary>
    /// Gets or sets the remove button aria label.
    /// </summary>
    [Parameter]
    public string RemoveButtonAria { get; set; }

    #endregion Parameters



}
