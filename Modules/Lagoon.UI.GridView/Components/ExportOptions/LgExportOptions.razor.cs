using Lagoon.UI.Application;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component ExportOptions.
/// </summary>
public partial class LgExportOptions : LgAriaComponentBase
{
    #region Fields

    /// <summary>
    /// Export options
    /// </summary>
    private ExportOptions _exportOption;

    /// <summary>
    /// LgModal ref
    /// </summary>
    private LgModal _lgModal;

    #endregion        

    #region Parameters

    /// <summary>
    /// Export provider manager
    /// </summary>
    [Parameter]
    public ExportProviderManager ExportProviderManager { get; set; }

    /// <summary>
    /// Export event
    /// </summary>
    [Parameter]
    public EventCallback<GridViewExportEventArgs> OnExport { get; set; }

    #endregion Parameters

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        IExportProvider defaultExportProvider = ExportProviderManager.FirstOrDefault();
        _exportOption = new() {
            ExportColumnsMode = (GridViewBehaviour.Options.ExportDisplayedColumns ? ExportColumnsMode.DisplayedColumns : ExportColumnsMode.AllColumns),
            ExportRowMode = (GridViewBehaviour.Options.ExportWithFilters ? ExportRowMode.FilteredRows : ExportRowMode.AllRows),
            ExportProvider = defaultExportProvider,
            ExportProviderId = defaultExportProvider.Id
        };
    }

    /// <summary>
    /// Export datagrid data to a file.
    /// </summary>
    /// <returns></returns>
    private async Task ExportAsync()
    {
        if (OnExport.HasDelegate)
        {
            await OnExport.TryInvokeAsync(App, new(_exportOption));
            await _lgModal.CloseAsync();
        }
    }

    /// <summary>
    /// Invoked to display modal
    /// </summary>
    /// <returns></returns>
    public Task ShowAsync()
    {
        return _lgModal.ShowAsync();
    }

    /// <summary>
    /// Update ExportProvider value from selection
    /// </summary>
    /// <param name="e">Event args</param>
    /// <returns></returns>
    public void SetProvider(ChangeEventArgs e)
    {
        _exportOption.ExportProvider = ExportProviderManager.Where(x => x.Id == (string)e.Value).FirstOrDefault();
    }

    #endregion methods
}
