using Demo.Client.Services;
using NavigationModeService = Lagoon.UI.Demo.Pages.NavigationModeService;

namespace Demo.Client.Shared;

public partial class App : IDisposable
{
    #region fields

    /// <summary>
    /// Tabbed navigation field.
    /// </summary>
    public bool _isTabbed;

    /// <summary>
    /// Indicate if the component have been disposed.
    /// </summary>
    private bool _disposedValue;

    #endregion

    #region dependencies injection

    /// <summary>
    /// Navigation mode handler.
    /// </summary>
    [Inject]
    public NavigationModeService NavigationMode { get; set; }

    #endregion

    #region initialisation and dispose

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavigationMode.OnNavigationModeChanged += OnNavigationModeChanged;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                NavigationMode.OnNavigationModeChanged -= OnNavigationModeChanged;
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region methods

    private void OnNavigationModeChanged()
    {
        StateHasChanged();
    }

    #endregion
}
