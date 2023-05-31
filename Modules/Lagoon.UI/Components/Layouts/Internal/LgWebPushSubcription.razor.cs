namespace Lagoon.UI.Components.Internal;


/// <summary>
/// Component used to display the WebPush subscription popup
/// </summary>
public partial class LgWebPushSubcription : LgComponentBase
{

    #region fields

    private bool _isNotifUiSubscribeVisible = false;
    private bool _hasActiveSubscription = false;
    private bool _navigatorBlocked;
    private string _localStorageKey;

    private bool IsVapidKeyAvailable
    {
        get
        {
            return !string.IsNullOrEmpty(App.Configuration["App:VapidPublicKey"]);
        }
    }

    private bool _isWebPushAvailable;

    #endregion

    #region initialization / destruction

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        App.OnDisplayWebPushSubscription += OnDisplayWebPushSubscriptionAsync;
        // LocalStorage key used to remember if the user has accepted or not the WebPush notification
        // rq: not deleted when disconnecting to be able to re-subscribe silently on reconnect
        _localStorageKey = App.GetLocalStorageKey("WebPushSubscribeState");
        _hasActiveSubscription = await JS.HasSubscribedToWebPushNotificationAsync();
        _isWebPushAvailable = IsVapidKeyAvailable && JS.IsServiceWorkerActive();
        if (!IsVapidKeyAvailable)
        {
            // There is no VAPID key, so we can't subscribe to WebPush notification
            _isNotifUiSubscribeVisible = false;
        }
        else if (!JS.IsServiceWorkerActive())
        {
            Console.WriteLine("There is valid VAPID info but service-worker is not active. Unable to subscribe to WebPush notification.");
            // no active service-worker so we can't subscribe to WebPush notification
            _isNotifUiSubscribeVisible = false;
        }
        else
        {
            // Check if user has explicitly accepted/refused the notification
            if (App.LocalStorage.TryGetItem(_localStorageKey, out bool? hasAcceptedNotif))
            {             
                if (hasAcceptedNotif.HasValue)
                {
                    // the user has already accepted notification and the browser allow notification:
                    // make the subscription silently
                    if (hasAcceptedNotif.Value && JS.IsPushNotificationGranted())
                    {
                        // rq: don't wait for response here to avoid loading latency
                        _ = SubscribeToNotifAsync();
                        _isNotifUiSubscribeVisible = false;
                    }
                }
                else
                {
                    // show the notification subscribe popup
                    _isNotifUiSubscribeVisible = true;
                }
            }
            else
            {
                _isNotifUiSubscribeVisible = true;
            }
        }
    }

    /// <summary>
    /// Free resources
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        App.OnDisplayWebPushSubscription -= OnDisplayWebPushSubscriptionAsync;
    }

    #endregion

    /// <summary>
    /// User has accepted the WebPush notification. Try to subscribe and send info to server-side
    /// </summary>
    /// <returns></returns>
    private async Task SubscribeToNotifAsync()
    {
        try
        {
            // Subscription to WebPush notification and send endpoint to server-side
            if (await App.SubscribeToPushNotificationAsync())
            {
                // Set a flag to remain that the user has accepted notification
                App.LocalStorage.SetItem(_localStorageKey, true);
                _isNotifUiSubscribeVisible = false;
            }
            else
            {
                ShowWarning("#lblWebPushNavigatorBlocked");
                _navigatorBlocked = true;
            }
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// User has refused the WebPush notification, hide the registration ui and remember user choice
    /// </summary>
    /// <returns></returns>
    private Task RejectNotifAsync()
    {
        try
        {
            // Set a flag to remain that the user has refused notification
            App.LocalStorage.SetItem(_localStorageKey, false);
            _isNotifUiSubscribeVisible = false;
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Hide the popup
    /// </summary>
    private void Close()
    {
        _isNotifUiSubscribeVisible = false;
    }

    /// <summary>
    /// Unsubcribe user to WebPush notification, remember user choice and close popup
    /// </summary>
    private async Task UnsubscribeToNotifAsync()
    {
        try
        {
            // Subscription to WebPush notification and send endpoint to server-side
            await App.UnsubscribeToPushNotificationAsync();
            // Set a flag to remain that the user has refused notification
            App.LocalStorage.SetItem<bool?>(_localStorageKey, null);
            _isNotifUiSubscribeVisible = false;
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Display WebPush notification popup on demand. <see cref="Lagoon.UI.Application.LgApplication.DisplayWebPushSubscriptionPopupAsync" />
    /// </summary>
    private async Task OnDisplayWebPushSubscriptionAsync()
    {
        _isWebPushAvailable = IsVapidKeyAvailable && JS.IsServiceWorkerActive();
        _hasActiveSubscription = await JS.HasSubscribedToWebPushNotificationAsync();
        _isNotifUiSubscribeVisible = true;
        StateHasChanged();
    }

}
