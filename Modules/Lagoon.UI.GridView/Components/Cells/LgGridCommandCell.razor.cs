using Lagoon.UI.Application;

namespace Lagoon.UI.Components;

/// <summary>
/// Command cell
/// </summary>
public class LgGridCommandCell<TItem> : LgGridBaseCell<TItem>
{
    #region methods

    ///<inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            if (!IsAdd && Column.CanEdit || IsAdd && Column.CanAdd)
            {
                // Get Column data                
                builder.OpenComponent<LgToolbar>(1);
                builder.AddAttribute(3, nameof(LgToolbar.ChildContent), (RenderFragment)((_builder) =>
                {
                    LgGridCommandColumn column = (LgGridCommandColumn)Column;
                    // First button
                    if (!string.IsNullOrEmpty(column.CommandName))
                    {
                        _builder.OpenComponent<LgToolbarGroup>(10);
                        _builder.AddAttribute(11, nameof(LgToolbarGroup.ChildContent), (RenderFragment)((_builder2) =>
                        {
                            _builder2.OpenComponent<LgToolbarButton>(12);
                            _builder2.AddAttribute(13, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                            _builder2.AddAttribute(14, nameof(LgToolbarButton.Text), column.ButtonText);
                            _builder2.AddAttribute(15, nameof(LgToolbarButton.AriaLabel), column.ButtonAriaLabel);
                            _builder2.AddAttribute(16, nameof(LgToolbarButton.IconName), column.IconName);
                            _builder2.AddAttribute(17, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(column.CommandName)));
                            _builder2.AddAttribute(18, nameof(LgToolbarButton.Tooltip), column.CommandTooltip);
                            _builder2.AddAttribute(19, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                            _builder2.AddAttribute(20, nameof(LgToolbarButton.ConfirmationMessage), column.ConfirmationMessage);
                            _builder2.CloseComponent();
                            // Command menu
                            if (column.Commands.Any())
                            {
                                _builder2.OpenComponent<LgToolbarMenu>(22);
                                _builder2.AddAttribute(23, nameof(LgToolbarMenu.ChildContent), (RenderFragment)((_builder3) =>
                                {
                                    _builder3.OpenRegion(30);
                                    foreach (LgGridCommand command in column.Commands)
                                    {
                                        _builder3.OpenComponent<LgToolbarButton>(1);
                                        _builder3.AddAttribute(2, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                                        _builder3.AddAttribute(3, nameof(LgToolbarButton.Text), command.Text);
                                        _builder3.AddAttribute(4, nameof(LgToolbarButton.AriaLabel), command.AriaLabel);
                                        _builder3.AddAttribute(5, nameof(LgToolbarButton.IconName), command.IconName);
                                        _builder3.AddAttribute(6, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(command.CommandName)));
                                        _builder3.AddAttribute(7, nameof(LgToolbarButton.Tooltip), command.Tooltip);
                                        _builder3.AddAttribute(8, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                                        _builder3.AddAttribute(9, nameof(LgToolbarButton.ConfirmationMessage), command.ConfirmationMessage);
                                        _builder3.CloseComponent();
                                    }
                                    _builder3.CloseRegion();
                                }));
                                _builder2.AddAttribute(24, nameof(LgToolbarMenu.AriaLabel), "#GridViewCommandListTitle");
                                _builder2.CloseComponent();
                            }
                        }));
                        _builder.CloseComponent();
                    }
                }));
                builder.CloseComponent();
            }                
        };
    }

    /// <inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("gridview-col-command");
        builder.AddIf(Column is LgGridEditColumn, "gridview-col-edit-command");
    }

    /// <summary>
    /// Propagate command click
    /// </summary>
    protected Task CommandClickAsync(string command)
    {
        // Command click event            
        return GridView.CommandClickAsync(command, Row);
    }

    #endregion
}
