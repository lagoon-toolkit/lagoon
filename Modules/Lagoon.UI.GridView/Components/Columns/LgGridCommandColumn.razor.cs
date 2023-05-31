namespace Lagoon.UI.Components;


/// <summary>
/// Gridview command column 
/// </summary>
public partial class LgGridCommandColumn : LgGridBaseNoValueColumn
{
    #region fields

    /// <summary>
    /// Unique key of the command column
    /// </summary>
    private const string DEFAULT_UNIQUE_KEY = "_DEFCMD";

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets content of the tag
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets aria-label of the button
    /// </summary>
    [Parameter]
    public string ButtonAriaLabel { get; set; }

    /// <summary>
    /// Gets or sets text of the button
    /// </summary>
    [Parameter]
    public string ButtonText { get; set; }

    /// <summary>
    /// Gets or sets command on the button
    /// </summary>
    [Parameter]
    public string CommandName { get; set; }

    /// <summary>
    /// Gets or sets icon of the button
    /// </summary>
    [Parameter]
    public string IconName { get; set; } = IconNames.All.PencilFill;

    /// <summary>
    /// Gets or sets the command tooltip
    /// </summary>
    [Parameter]
    public string CommandTooltip { get; set; }

    /// <summary>
    /// Gets or sets list of command on the row
    /// </summary>
    [Parameter]
    public List<LgGridCommand> Commands { get; set; } = new List<LgGridCommand>();

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        return typeof(LgGridCommandCell<>).MakeGenericType(itemType);
    }

    ///<inheritdoc/>
    protected override string OnGetQuickKey(ParameterView parameters)
    {
        return base.OnGetQuickKey(parameters) ?? parameters.GetValueOrDefault<string>(nameof(CommandName)) ?? DefaultUniqueKey();
    }

    ///<inheritdoc/>
    protected override string OnGetUniqueKey()
    {
        return base.OnGetUniqueKey() ?? CommandName ?? DefaultUniqueKey();
    }

    /// <summary>
    /// Get a constant unique key, if it's undifined.
    /// </summary>
    /// <returns></returns>
    protected virtual string DefaultUniqueKey()
    {
        return DEFAULT_UNIQUE_KEY;
    }

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        State.AllowFilter = false;
        State.AllowSort = false;
        State.AllowHide = false;
        State.Visible = true;
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
    }

    #endregion
}
