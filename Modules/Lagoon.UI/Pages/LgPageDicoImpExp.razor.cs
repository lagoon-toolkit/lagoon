using Lagoon.Internal;
using System.Net.Http.Json;

namespace Lagoon.UI.Pages;


/// <summary>
/// 
/// </summary>
[Route(ROUTE)]
[Authorize()]
public partial class LgPageDicoImpExp : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "lg/dico-manager";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgDicoManagement", IconNames.All.Book);
    }

    #endregion

    #region Private variables

    /// <summary>
    /// 
    /// </summary>
    private bool _createBackup = true;

    #endregion

    #region Initialization

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetTitleAsync(Link());
    }

    #endregion

    #region Methods

    /// <summary>
    /// Export all dico keys into a csv file
    /// </summary>
    /// <returns></returns>
    private async Task ExportDicoAsync()
    {
        try
        {
            using (WaitingContext wc = GetNewWaitingContext())
            {
                string dicoClient = App.DicoToCsv("Client", true, await Http.TryGetAsync<IEnumerable<string>>(Routes.DICO_KEYS_URI));
                HttpResponseMessage response = await Http.PostAsJsonAsync($"{Routes.DICO_CSV_URI}?includeHeader={_createBackup}", App.GetDicoKeys(), wc.CancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    string dicoServer = await response.Content.ReadAsStringAsync();
                    App.SaveAsFile("Dico.csv", dicoClient + dicoServer, System.Text.Encoding.UTF8);
                }
                else
                {
                    throw new UserException($"An error occured. Status Code:{response.StatusCode}. Reason{response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

}