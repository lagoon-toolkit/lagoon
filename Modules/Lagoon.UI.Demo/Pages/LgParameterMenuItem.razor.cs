using Lagoon.UI.Components;
using Lagoon.UI.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// Parameter page menu link
/// </summary>
public class LgParameterMenuItem : LgMenuItem
{
    #region dependency injection

    /// <summary>
    /// Window manager
    /// </summary>
    /// <value></value>
    [Inject]
    public WindowManager WindowManager { get; set; } = default!;

    #endregion

    #region parameters

    /// <summary>
    /// is tabbed boolean
    /// </summary>
    [Parameter]
    public bool IsTabbed { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var link = LgPageParameter.Link();
        Text = link.Title;
        IconName = IconNames.All.Tools;
        OnClick = EventCallback.Factory.Create<ActionEventArgs>(this, () =>
        {
            WindowManager.OpenModal(link.Uri, null, null, ModalSize.Medium);
        });
    }

    #endregion
}
