namespace Lagoon.UI.Components;

/// <summary>
/// Container for modal windows.
/// </summary>
public partial class LgModalContainer : LgComponentBase
{
    #region private fields
    private WindowManager _windowManager;
    #endregion

    #region Properties
    /// <summary>
    /// Render modals
    /// </summary>
    /// <value></value>
    public RenderFragment ChildContent => builder => {
        WindowManager.Render(builder);
    };

    #endregion

    #region Dependencies
    /// <summary>
    /// Window Manager
    /// </summary>
    /// <value></value>
    [Inject]
    private WindowManager WindowManager
    {
        get
        {
            return _windowManager;
        }
        set
        {
            _windowManager = value;
            _windowManager.OnModalAdded += ModalAdded;
            _windowManager.OnModalRemoved += ModalRemoved;
            _windowManager.OnModalChanged += ModalChanged;
        }
    }
    #endregion

    #region methods
    /// <summary>
    /// Modal opened
    /// </summary>
    private void ModalAdded()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Modal closed
    /// </summary>
    private void ModalRemoved()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Modal changed
    /// </summary>
    private void ModalChanged()
    {
        StateHasChanged();
    }
    #endregion
}
