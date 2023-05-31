using Microsoft.AspNetCore.Mvc.RazorPages;
using Lagoon.Model.Context;
using Microsoft.EntityFrameworkCore;
using Lagoon.Server.Services;

namespace Lagoon.Server.Areas.App.Pages.Config;

/// <summary>
/// Page used to display application initialization option
/// </summary>
[AllowAnonymous]
public class ConfigModel : PageModel
{

    #region fields

    private readonly ILgApplication _app;
    private readonly ILgApplicationDbContext _dbContext;
    private readonly IServiceProvider _sp;
    private readonly IConfiguration _config;
    private readonly ILgOpenIddictManager _openIddictManager;

    #endregion

    #region models

    /// <summary>
    /// Error message
    /// </summary>
    [BindProperty]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Show/hide the button to launch openiddict initialisation
    /// </summary>
    [BindProperty]
    public bool ShowInitApp { get; set; }

    /// <summary>
    /// Show/hide the button to launch pending migrations
    /// </summary>
    [BindProperty]
    public bool ShowApplyPendingMigrations { get; set; }

    /// <summary>
    /// Flag to handle if the user is authenticated or not
    /// </summary>
    [BindProperty]
    public bool IsUserAuthenticated { get; set; } = false;

    /// <summary>
    /// Flag to handle if the user is authenticated or not
    /// </summary>
    [BindProperty]
    public List<string> RedirectUris { get; set; } = new();

    /// <summary>
    /// Flag to handle if the user is authenticated or not
    /// </summary>
    [BindProperty]
    public bool IsCurrentPublicUrlMissing { get; set; } = false;

    #endregion

    #region initialization

    /// <summary>
    /// Page initialisation
    /// </summary>
    /// <param name="app">Lagoon application</param>
    /// <param name="dbContext">Db context</param>
    /// <param name="sp">Ser</param>
    /// <param name="openIddictManager"></param>
    public ConfigModel(ILgApplication app, ILgApplicationDbContext dbContext, IServiceProvider sp, ILgOpenIddictManager openIddictManager)
    {
        _app = app;
        _dbContext = dbContext;
        _sp = sp;
        _config = app.Configuration;
        _openIddictManager = openIddictManager;
    }

    // Init flags used by the view
    private async Task InitAsync()
    {
        ShowApplyPendingMigrations = _dbContext.Database.GetPendingMigrations().Any();
        if (!ShowApplyPendingMigrations)
        {
            // rq: we can't be authenticated when 1st app registration ...
            //if (IsUserAuthenticated) 
            {
                // rq: check init only if there is no pending migration detected since
                // the openiddict app manager need to access db
                string clientAppId = $"{_app.ApplicationInformation.RootName}.Client";
                RedirectUris = await _openIddictManager.GetRedirectUrisAsync(_sp, clientAppId);
                string currentPublicUrl = _config["Lagoon:PublicUrl"];
                if (!string.IsNullOrEmpty(currentPublicUrl))
                {
                    IsCurrentPublicUrlMissing = true;
                    foreach (string uri in RedirectUris)
                    {
                        if (uri.Contains(currentPublicUrl))
                        {
                            IsCurrentPublicUrlMissing = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region forms actions

    /// <summary>
    /// Manage button visibility according to app state
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnGetAsync()
    {
        string pageToken = _config["Lagoon:ConfigPageToken"];
        // rq: don't provide access to this page if there is no token defined in appSetting
        IsUserAuthenticated = !string.IsNullOrEmpty(pageToken) && Request.QueryString.HasValue && !string.IsNullOrEmpty(pageToken) && Request.QueryString.Value.Contains($"token={pageToken}");
        if (!IsUserAuthenticated)
        {
            return StatusCode(404);
            //throw new UnauthorizedAccessException();
        }
        //var userAuth = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        //if (userAuth.Succeeded) // && userAuth.Principal.IsInRole()
        //{
        //    IsUserAuthenticated = userAuth.Principal.Identity.IsAuthenticated;
        //}
        await InitAsync();
        return Page();

    }

    /// <summary>
    /// Apply all pending migrations
    /// </summary>
    public async Task<IActionResult> OnPostApplyPendingMigrationAsync()
    {
        try
        {
            await _app.UpdateDatabaseAsync(_sp, CancellationToken.None);
            await InitAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _app.TraceException(ex);
        }
        return Page();
    }

    /// <summary>
    /// Relaunch Db intialization
    /// </summary>
    public async Task<IActionResult> OnPostOpeniddictRegistrationAsync()
    {
        try
        {
            if (_app is ILgAuthApplication authApp)
            {
                await authApp.InitializeOpenIddictAsync(_sp, CancellationToken.None);
            }
            await InitAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _app.TraceException(ex);
        }
        return Page();
    }

    #endregion

}