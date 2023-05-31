namespace Lagoon.UI.Components.Internal;

internal class ActionContext<TActionEventArgs> where TActionEventArgs : ActionEventArgs
{
    #region fields

    private readonly LgComponentBase _component;
    private readonly string _confirmation;
    private readonly string _uri;
    private readonly Func<WaitingContext, TActionEventArgs> _newArgCallback;
    private readonly EventCallback<TActionEventArgs> _onAction;
    private readonly string _target;

    #endregion

    #region constructors

    internal ActionContext(LgComponentBase component, EventCallback<TActionEventArgs> onAction, Func<WaitingContext, TActionEventArgs> newArgCallback, 
        string confirmation = null, string uri = null, string target = null)
    {
        _onAction = onAction;
        _newArgCallback = newArgCallback;
        _component = component;
        _confirmation = confirmation;
        _uri = uri;
        _target = target;
    }

    #endregion

    #region methods

    internal async Task ExecuteAsync()
    {
        if (!string.IsNullOrEmpty(_confirmation))
        {
            _component.ShowConfirm(_confirmation, ExecuteWithoutConfirmationAsync);
        }
        else
        {
            await ExecuteWithoutConfirmationAsync();
        }
    }

    private async Task ExecuteWithoutConfirmationAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_uri))
            {
                if (_onAction.HasDelegate)
                {
                    using (WaitingContext wc = _component.GetNewWaitingContext())
                    {
                        await _onAction.InvokeAsync(_newArgCallback(wc));
                    }
                }
            }
            else if (string.IsNullOrEmpty(_target))
            {
                // Navigate to the URI
                _component.App.NavigateTo(_uri);
            }
            else
            {
                // Open the URI into a new window
                await _component.App.OpenWindowAsync(_uri, _target);
            }
        }
        catch (Exception ex)
        {
            _component.ShowException(ex);
        }
    }

    #endregion

}
