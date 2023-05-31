using Lagoon.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// page for font book
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class LgPageFontBook : LgPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    public const string ROUTE = "lg/font-book";

    private readonly string _uneVar = "a";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgFontBook", IconNames.All.FileFont);
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

