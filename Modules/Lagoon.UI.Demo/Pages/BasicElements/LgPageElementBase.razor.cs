using Lagoon.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Lagoon.UI.Demo.Pages.BasicElements;

[AllowAnonymous()]
[Route(ROUTE)]
[Route(ROUTE + "/{ActiveTab}")]
public partial class LgPageElementBase 
{ 
    /// <summary>
    /// Gets or sets the current tab.
    /// </summary>
    [Parameter]
    public string ActiveTab { get; set; }

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/elementBase";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgElementBase");
    }

    #endregion

    #region Initialization

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());

    }

    #endregion
}
