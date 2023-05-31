using Lagoon.Internal;
using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lagoon.UI.Components;

/// <summary>
/// Notification service
/// </summary>
/// <typeparam name="TItem">Item data type (ViewModel)</typeparam>
internal class ClientNotificationService<TItem> : INotificationService<TItem> where TItem : NotificationVmBase
{

    #region dependencies injections

    // Http client
    private HttpClient _http;

    // Navigation manager
    private NavigationManager _navigationManager;

    // Hub connection to signalr
    private HubConnection _hubConnection;

    // Js runtime
    private readonly IJSRuntime _js;

    // Access token for hub connection
    private IAccessTokenProvider _accessTokenProvider;

    // The name of the Notification Data Store
    private static string _notificationDataStoreKey;

    #endregion

    #region fields

    /// <summary>
    /// Gets or sets the controller name 
    /// </summary>
    private const string ControllerName = Routes.NOTIFICATIONS_ROUTE;

    #endregion

    #region constructor

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="js">Js Runtime</param>
    /// <param name="http">Http client</param>
    /// <param name="navigationManager">Navigation manager</param>
    /// <param name="accessTokenProvider">Access token provider used for hub connection</param>
    /// <param name="app">The current application.</param>
    public ClientNotificationService(IJSRuntime js, HttpClient http, NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider, LgApplication app)
    {
        _js = js;
        _http = http;
        _navigationManager = navigationManager;
        _accessTokenProvider = accessTokenProvider;
        _notificationDataStoreKey = GetDataStoreKey(app);
    }

    /// <summary>
    /// Add the key to delete to the notifications when the user log out.
    /// </summary>
    /// <param name="app">The application.</param>
    internal static void RegisterToSignoutManager(LgApplication app)
    {
        app.SignOutCleaner.AddIndexDbKey(GetDataStoreKey(app));
    }

    /// <summary>
    /// Get the local storage key used to save tabs states.
    /// </summary>
    /// <param name="app">The application.</param>
    private static string GetDataStoreKey(LgApplication app)
    {
        return app.GetLocalStorageKey("NotificationDataStore");
    }

    #endregion

    #region hub management

    private Task _initializeTask;

    /// <summary>
    /// Service initialization
    /// </summary>
    /// <returns></returns>
    internal Task InitializeAsync()
    {
        if (_initializeTask is null)
        {
            lock (_lock)
            {
                _initializeTask ??= InitHubConnectAsync();
            }
        }
        return _initializeTask;
    }

    private static object _lock = new();

    /// <summary>
    /// Init hub connection
    /// </summary>
    private async Task InitHubConnectAsync()
    {
        await Task.Delay(10000);

        await SyncRemoteDbToIndexedDbAsync();

        _hubConnection = new HubConnectionBuilder()
         .WithUrl(_navigationManager.ToAbsoluteUri("hubs/notifhub"), options =>
         {
             options.AccessTokenProvider = async () =>
             {
                 AccessTokenResult accessTokenResult = await _accessTokenProvider.RequestAccessToken();
                 accessTokenResult.TryGetToken(out AccessToken accessToken);
                 return accessToken.Value;
             };
         })
         .WithAutomaticReconnect()
        .AddMessagePackProtocol()
         .Build();

        // Refesh notification list
        _hubConnection.On("OnRefreshNotification", SyncRemoteDbToIndexedDbAsync);

        _hubConnection.On("OnRemoveNotification", async (Guid userNotificationId) =>
        {
            await DeleteLocalNotificationAsync(userNotificationId);
            await NotificationChangeAsync();
        });
        _hubConnection.On("OnRemoveNotifications", async (List<Guid> userNotificationIds) =>
        {
            await DeleteLocalNotificationAsync(userNotificationIds);
            await NotificationChangeAsync();
        });

        // On reconnected to hub
        _hubConnection.Reconnected += async connectionId =>
        {
            List<NotificationPendingActionVm> pendingActions = await GetPendingActionsAsync();

            if (pendingActions != null)
            {
                // Send pending actions from indexeddb to server
                await _http.TryPostAsync<List<NotificationPendingActionVm>>($"{ControllerName}/SendPendingActions", pendingActions);

                // Remove pending action from indexeddb
                await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.pendingActionsDeleteAll", _notificationDataStoreKey);
            }
        };
        await _hubConnection.StartAsync();
    }

    /// <summary>
    /// Return hub connection for notification service
    /// </summary>
    /// <returns></returns>
    internal HubConnection GetHubConnection()
    {
        return _hubConnection;
    }

    #endregion

    #region events

    /// <summary>
    /// Event raise the list of notification is updated.
    /// </summary>
    public event Action<List<TItem>> OnNotificationChange;

    #endregion

    #region methods

    /// <summary>
    /// Synchronize remote database items to indexedDb notification store
    /// </summary>
    /// <returns></returns>
    internal async Task SyncRemoteDbToIndexedDbAsync()
    {
        List<TItem> databaseItems;
        // Check existing data into indexedDb store
        List<TItem> localItems = await GetNotificationsAsync();
        if (localItems != null && localItems.Count > 0)
        {
            // Get notification item from indexedDb with the max updatedate
            TItem lastItem = localItems.OrderByDescending(x => x.UpdateDate).First();

            // Get database notifications with updatedate later or equals to the notification found previously (lastItem)
            databaseItems = await _http.TryPostAsync<TItem, List<TItem>>($"{ControllerName}/GetNotifications", lastItem);
            // Save notifications into indexedDb notification store
            if (databaseItems != null)
            {
                await UpdateLocalNotificationAsync(databaseItems);
            }
        }
        else
        {
            // Get database notifications
            databaseItems = await _http.TryGetAsync<List<TItem>>($"{ControllerName}/GetNotifications");

            // Save notifications into indexedDb notification store
            if (databaseItems != null)
            {
                await CreateLocalNotificationAsync(databaseItems);
            }
        }
        // Refresh notificaton list
        await NotificationChangeAsync();
    }

    /// <summary>
    /// Retrieve notification on change
    /// </summary>
    private async Task NotificationChangeAsync()
    {
        List<TItem> notifications = await GetNotificationsAsync();
        OnNotificationChange.Invoke(notifications);
    }

    /// <summary>
    /// Mark as read or unread a notification item.
    /// </summary>
    /// <param name="notificationUserId">A notification identifier.</param>
    /// <param name="isRead">Indicates whether the notification is marked as read.</param>
    public async Task SetReadStateItemAsync(Guid notificationUserId, bool isRead)
    {
        try
        {
            // Online
            await _http.GetAsync($"{ControllerName}/UpdateNotificationReadState/{notificationUserId}/{isRead}");
        }
        catch (LgHttpFetchException)
        {
            // Offline
            // Update pipeline and local notification for reconnection
            await UpdateNotificationReadingOfflineAsync(notificationUserId, isRead);
            await NotificationChangeAsync();
        }
    }

    /// <summary>
    /// Mark as read or unread a list of notification items.
    /// </summary>
    /// <param name="notificationUserIds">A list of notification identifiers.</param>
    /// <param name="isRead">Indicates whether the notification is marked as read.</param>
    public async Task SetReadStateItemsAsync(IEnumerable<Guid> notificationUserIds, bool isRead)
    {
        try
        {
            // Online
            await _http.TryPostAsync($"{ControllerName}/UpdateNotificationReadState/{isRead}", notificationUserIds);
        }
        catch (LgHttpFetchException)
        {
            // Offline
            // Update pipeline and local notification for reconnection
            await UpdateNotificationReadingOfflineAsync(notificationUserIds, isRead);
            await NotificationChangeAsync();
        }
    }

    /// <summary>
    /// Remove a notification item.
    /// </summary>
    /// <param name="notificationUserId">A notification identifier.</param>
    public async Task DeleteItemAsync(Guid notificationUserId)
    {
        try
        {
            // Online
            await _http.TryDeleteAsync($"{ControllerName}/DeleteUserNotification/{notificationUserId}");
        }
        catch (LgHttpFetchException)
        {
            // Offline
            // Update pipeline and local notification for reconnection
            await DeleteNotificationOfflineAsync(notificationUserId);
            await NotificationChangeAsync();
        }
    }
    /// <summary>
    /// Remove a list of notification items.
    /// </summary>
    /// <param name="notificationUserIds">A list of notification identifiers.</param>
    public async Task DeleteItemsAsync(IEnumerable<Guid> notificationUserIds)
    {
        try
        {
            // Online
            await _http.TryPostAsync($"{ControllerName}/DeleteUserNotification", notificationUserIds);
        }
        catch (LgHttpFetchException)
        {
            // Offline
            // Update pipeline and local notification for reconnection
            await DeleteNotificationOfflineAsync(notificationUserIds);
            await NotificationChangeAsync();
        }
    }

    #endregion

    #region IndexedDb management (Offline mode)

    /// <summary>
    /// Add notificatons into indexedDb notification store
    /// </summary>
    /// <param name="notifications">List of notifications</param>
    /// <returns></returns>
    private async Task CreateLocalNotificationAsync(List<TItem> notifications)
    {
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.syncNotification", _notificationDataStoreKey, notifications);
    }

    /// <summary>
    /// Update list notificatons into indexedDb notification store
    /// </summary>
    /// <param name="notifications">List of notifications</param>
    /// <returns></returns>
    private async Task UpdateLocalNotificationAsync(List<TItem> notifications)
    {
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.syncNotification", _notificationDataStoreKey, notifications);
    }

    /// <summary>
    /// Delete a notification from indexedDb notification store
    /// </summary>
    /// <param name="userNotificationId">Notification user id</param>
    /// <returns></returns>
    private async Task DeleteLocalNotificationAsync(Guid userNotificationId)
    {
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.deleteNotification", _notificationDataStoreKey, userNotificationId);
    }

    /// <summary>
    /// Delete a notification list from indexedDb notification store
    /// </summary>
    /// <param name="userNotificationIds">List of Notification user id</param>
    /// <returns></returns>
    private async Task DeleteLocalNotificationAsync(List<Guid> userNotificationIds)
    {
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.deleteNotifications", _notificationDataStoreKey, userNotificationIds);
    }

    /// <summary>
    /// Retrieve a notifications list from the indexedDb notification store.
    /// </summary>
    /// <returns>List of notification item (viewmodel).</returns>
    public async Task<List<TItem>> GetNotificationsAsync()
    {
        return await _js.InvokeAsync<List<TItem>>("Lagoon.LgNotificationDataStore.getNotifications", _notificationDataStoreKey);
    }

    /// <summary>
    /// Retrive a list of pending action 
    /// </summary>
    /// <returns>List of pending action</returns>
    private async Task<List<NotificationPendingActionVm>> GetPendingActionsAsync()
    {
        return await _js.InvokeAsync<List<NotificationPendingActionVm>>("Lagoon.LgNotificationDataStore.getPendingActions", _notificationDataStoreKey);
    }

    /// <summary>
    /// Update a notification in offline mode
    /// Add an update pending action
    /// </summary>
    /// <param name="notificationUserId">Notification user id</param>
    /// <param name="isRead">Is read flag</param>
    /// <returns></returns>
    private async Task UpdateNotificationReadingOfflineAsync(Guid notificationUserId, bool isRead)
    {
        NotificationPendingActionVm pendingNotificationAction = new()
        {
            Action = NotificationPendingAction.Update,
            IsRead = isRead,
            NotificationUserId = notificationUserId,
            UpdateDate = DateTime.Now
        };
        // Add pending action to send on recoonect to server and set 
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.pendingActionsUpdateNotification", _notificationDataStoreKey, pendingNotificationAction);
    }

    /// <summary>
    /// Update a list of notification in offline mode
    /// Add an update pending action
    /// </summary>
    /// <param name="notificationUserIds">List of Notification user id</param>
    /// <param name="isRead">Is read flag</param>
    /// <returns></returns>
    private async Task UpdateNotificationReadingOfflineAsync(IEnumerable<Guid> notificationUserIds, bool isRead)
    {
        List<NotificationPendingActionVm> pendings = new();
        foreach (Guid notificationUserId in notificationUserIds)
        {
            pendings.Add(new NotificationPendingActionVm()
            {
                Action = NotificationPendingAction.Update,
                IsRead = isRead,
                NotificationUserId = notificationUserId,
                UpdateDate = DateTime.Now
            });
        }
        // Add pending actions to send on recoonect to server and set 
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.pendingActionsUpdateNotifications", _notificationDataStoreKey, pendings);
    }

    /// <summary>
    /// Remove a notificaton in offline mode
    /// Add a delete pending action
    /// </summary>
    /// <param name="notificationUserId">User notification id</param>
    /// <returns></returns>
    private async Task DeleteNotificationOfflineAsync(Guid notificationUserId)
    {
        NotificationPendingActionVm pendingNotificationAction = new()
        {
            Action = NotificationPendingAction.Delete,
            IsRead = false,
            NotificationUserId = notificationUserId,
            UpdateDate = DateTime.Now
        };
        // Add pending action to send on recoonect to server and set 
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.pendingActionsDeleteNotification", _notificationDataStoreKey, pendingNotificationAction);
    }

    /// <summary>
    /// Remove a list of notificaton in offline mode
    /// Add a delete pending action
    /// </summary>
    /// <param name="notificationUserIds">List of user notification id</param>
    /// <returns></returns>
    private async Task DeleteNotificationOfflineAsync(IEnumerable<Guid> notificationUserIds)
    {
        List<NotificationPendingActionVm> pendings = new();
        foreach (Guid notificationUserId in notificationUserIds)
        {
            pendings.Add(new NotificationPendingActionVm()
            {
                Action = NotificationPendingAction.Delete,
                IsRead = false,
                NotificationUserId = notificationUserId,
                UpdateDate = DateTime.Now
            });
        }
        // Add pending action to send on recoonect to server and set 
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.pendingActionsDeleteNotifications", _notificationDataStoreKey, pendings);
    }

    /// <summary>
    /// Fully delete the database from IndexedDb 
    /// </summary>
    /// <returns></returns>
    internal async Task DeleteDabaseAsync()
    {
        await _js.InvokeVoidAsync("Lagoon.LgNotificationDataStore.deleteDatabase", _notificationDataStoreKey);
    }

    #endregion

}
