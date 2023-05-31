using Lagoon.UI.Application;

namespace Lagoon.UI.Helpers;

/// <summary>
/// Window manager service
/// </summary>
public class WindowManager
{
    #region private fields

    /// <summary>
    /// Modal list
    /// </summary>
    private readonly Dictionary<string, DynamicModal> _modalList = new();

    #endregion

    #region Dependencies

    /// <summary>
    /// The main application.
    /// </summary>
    private LgApplication _app;

    #endregion

    #region Events

    /// <summary>
    /// Event rising when modal is added
    /// </summary>
    /// <value></value>
    internal Action OnModalAdded { get; set; }

    /// <summary>
    /// Event rising when modal is closed
    /// </summary>
    /// <value></value>
    internal Action OnModalRemoved { get; set; }

    /// <summary>
    /// Event rising when modal parameter has changed
    /// </summary>
    /// <value></value>
    internal Action OnModalChanged { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// The window manager..
    /// </summary>
    /// <param name="app">The main application.</param>
    public WindowManager(LgApplication app)
    {
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Add confirmation modal to build
    /// </summary>
    /// <param name="confirmMessage">Confirmation message</param>
    /// <param name="confirmCallback">Method call after confirmation</param>
    /// <param name="title">Modal title</param>
    /// <param name="cssClass">Css class</param>
    /// <param name="modalSize">Modal size</param>
    internal void OpenConfirm(string confirmMessage, Func<Task> confirmCallback, string title = null,
                            string cssClass = null, ModalSize modalSize = ModalSize.Medium)
    {
        string key = "confirm";
        DynamicModal modal = new()
        {
            Key = key,
            Title = title,
            CssClass = cssClass,
            ModalSize = modalSize,
            ConfirmMessage = confirmMessage,
            ConfirmCallback = confirmCallback,
            IsConfirm = true
        };
        if (_modalList.ContainsKey(key))
        {
            _modalList[key] = modal;
        }
        else
        {
            _modalList.Add(key, modal);
        }
        // Notify modal adding
        OnModalAdded?.Invoke();
    }

    /// <summary>
    /// Add modal to build
    /// </summary>
    /// <param name="uri">Route uri</param>
    /// <param name="title">Modal title</param>
    /// <param name="cssClass">Modal css class</param>
    /// <param name="modalSize">Modal size</param>
    public void OpenModal(string uri, string title = null, string cssClass = null, ModalSize modalSize = ModalSize.Medium)
    {
        // Get route from Uri
        RouteData routeData = _app.GetRouteData(uri);
        if (routeData is not null)
        {
            string key = LgRouter.GenerateTabKey(uri, "modal");
            DynamicModal dynModal = new()
            {
                Key = key,
                Title = title,
                CssClass = cssClass,
                ModalSize = modalSize,
                RouteData = routeData
            };
            if (_modalList.ContainsKey(key))
            {
                _modalList[key] = dynModal;
            }
            else
            {
                _modalList.Add(key, dynModal);
            }
        }
        // Notify modal adding
        OnModalAdded?.Invoke();
    }

    /// <summary>
    /// Remove modal on close
    /// </summary>
    /// <param name="key">Key of the modal</param>
    private void CloseModal(string key)
    {
        _modalList?.Remove(key);
    }

    /// <summary>
    /// Render modals
    /// </summary>
    /// <param name="builder">RenderTreeBuilder</param>
    internal void Render(RenderTreeBuilder builder)
    {
        if (_modalList != null)
        {
            int idx = 0;
            foreach (DynamicModal modal in EnumerateDynamicModals())
            {
                builder.OpenRegion(idx++);
                if (!modal.IsConfirm)
                {
                    builder.OpenComponent<LgModal>(1);
                }
                else
                {
                    builder.OpenComponent<LgConfirm>(1);
                }
                builder.AddAttribute(2, nameof(LgModal.Title), modal.Title);
                builder.AddAttribute(3, nameof(LgModal.CssClass), modal.CssClass);
                builder.AddAttribute(4, nameof(LgModal.ModalSize), modal.ModalSize);
                builder.AddAttribute(5, nameof(LgModal.DefaultVisible), modal.Show);
                builder.AddAttribute(6, nameof(LgModal.OnClose), EventCallback.Factory.Create<CloseModalEventArgs>(this, () =>
                {
                    modal.Show = false;
                    CloseModal(modal.Key);
                    // Notify modal removing
                    OnModalRemoved?.Invoke();
                }));
                builder.AddAttribute(7, nameof(LgModal.ChildContent), (RenderFragment)(subBuilder =>
                {
                    if (modal.RouteData != null)
                    {
                        subBuilder.OpenComponent<LgPageAuthorizeRouteView>(0);
                        subBuilder.AddAttribute(1, nameof(LgPageAuthorizeRouteView.RouteData), modal.RouteData);
                        subBuilder.CloseComponent();
                    }
                }));
                if (modal.IsConfirm)
                {
                    builder.AddAttribute(100, nameof(LgConfirm.ConfirmationMessage), modal.ConfirmMessage);
                    builder.AddAttribute(101, nameof(LgConfirm.ConfirmCallback), modal.ConfirmCallback);
                }
                builder.CloseComponent();
                builder.CloseRegion();
            }
        }
    }

    private IEnumerable<DynamicModal> EnumerateDynamicModals()
    {
        if (_modalList != null)
        {
            foreach (KeyValuePair<string, DynamicModal> modal in _modalList)
            {
                yield return modal.Value;
            }
        }
    }

    #endregion

}
