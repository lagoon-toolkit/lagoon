using Lagoon.UI.Application;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Cell for the edit column
/// </summary>
public class LgGridEditCell<TItem> : LgGridCommandCell<TItem>
{
    #region methods

    /// <inheritdoc/>
    protected override RenderFragment GetCellContent()
    {
        return builder =>
        {
            LgGridEditColumn column = (LgGridEditColumn)Column;
            string title = "";
            builder.OpenComponent<LgToolbar>(1);
            builder.AddAttribute(2, nameof(LgToolbar.ChildContent), (RenderFragment)((_builder) =>
            {
                _builder.OpenComponent<LgToolbarGroup>(10);
                _builder.AddAttribute(11, nameof(LgToolbarGroup.ChildContent), (RenderFragment)((_builder2) =>
                {
                    if (IsAdd)
                    {
                        // Add button
                        _builder2.OpenComponent<LgToolbarButton>(20);
                        _builder2.AddAttribute(21, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                        title = "GridViewAddSaveTooltip".Translate();
                        _builder2.AddAttribute(22, nameof(LgToolbarButton.IconName), IconNames.Save);
                        _builder2.AddAttribute(23, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(LgGridEditColumn.ADDCOMMAND)));
                        _builder2.AddAttribute(24, nameof(LgToolbarButton.Tooltip), title);
                        _builder2.AddAttribute(25, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                        _builder2.AddAttribute(26, nameof(LgButton.AriaLabel), title);
                        _builder2.CloseComponent();
                        // cancel button
                        _builder2.OpenComponent<LgToolbarButton>(50);
                        _builder2.AddAttribute(51, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                        title = "GridViewAddCancelAddTooltip".Translate();
                        _builder2.AddAttribute(52, nameof(LgToolbarButton.IconName), IconNames.All.X);
                        _builder2.AddAttribute(53, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(LgGridEditColumn.ADDCANCELCOMMAND)));
                        _builder2.AddAttribute(54, nameof(LgToolbarButton.Tooltip), title);
                        _builder2.AddAttribute(55, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                        _builder2.AddAttribute(56, nameof(LgButton.AriaLabel), title);
                        _builder2.CloseComponent();
                    }
                    else
                    {
                        _builder2.OpenComponent<LgToolbarButton>(30);
                        _builder2.AddAttribute(31, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                        if (Row.RowEditing)
                        {
                            // Save button
                            title = "GridViewEditSaveTooltip".Translate();
                            _builder2.AddAttribute(32, nameof(LgToolbarButton.IconName), IconNames.Save);
                            _builder2.AddAttribute(33, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(LgGridEditColumn.SAVECOMMAND)));
                        }
                        else
                        {
                            // Edit button                                        
                            title = "GridViewEditTooltip".Translate();
                            _builder2.AddAttribute(32, nameof(LgToolbarButton.IconName), IconNames.All.PencilFill);
                            _builder2.AddAttribute(33, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(LgGridEditColumn.EDITCOMMAND)));
                        }
                        _builder2.AddAttribute(34, nameof(LgToolbarButton.Tooltip), title);
                        _builder2.AddAttribute(35, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                        _builder2.AddAttribute(36, nameof(LgButton.AriaLabel), title);
                        _builder2.CloseComponent();
                        if (Row.RowEditing)
                        {
                            // Cancel button
                            string titleCancel = "GridViewEditCancelTooltip".Translate();
                            _builder2.OpenComponent<LgToolbarButton>(40);
                            _builder2.AddAttribute(41, nameof(LgToolbarButton.Kind), ButtonKind.Secondary);
                            _builder2.AddAttribute(42, nameof(LgToolbarButton.IconName), IconNames.All.X);
                            _builder2.AddAttribute(43, nameof(LgToolbarButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, () => CommandClickAsync(LgGridEditColumn.EDITCANCELCOMMAND)));
                            _builder2.AddAttribute(44, nameof(LgToolbarButton.Tooltip), titleCancel);
                            _builder2.AddAttribute(45, nameof(LgToolbarButton.TooltipPosition), GridViewBehaviour.Options.TooltipPosition);
                            _builder2.AddAttribute(46, nameof(LgToolbarButton.AriaLabel), titleCancel);
                            _builder2.CloseComponent();
                        }
                    }
                }));
                _builder.CloseComponent();
            }));

            builder.CloseComponent();
        };
    }

    #endregion
}
