using Lagoon.Hubs;
using Lagoon.Model.Context;
using Lagoon.Model.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Server.Application;


/// <summary>
/// Service used to manage eula data
/// </summary>
/// <typeparam name="TUser">Because of the dbContext we must have the IdentityUser type used by the application</typeparam>
/// <typeparam name="TVmNotification">User notification view model</typeparam>
/// <typeparam name="TNotification">Notification model</typeparam>
/// <typeparam name="TNotificationWithUser">NotificationUser model</typeparam>
/// <typeparam name="TKey">The type of the primary key for users.</typeparam>
public abstract class LgNotificationManagerBase<TUser, TVmNotification, TNotification, TNotificationWithUser, TKey> : ILgNotificationManager<TVmNotification, TNotification>
    where TUser : class, ILgIdentityUser
    where TVmNotification : NotificationVmBase, new()
    where TNotification : NotificationBase, new()
    where TNotificationWithUser : NotificationUserWithUserBase<TNotification, TUser, TKey>, new()
    where TKey : IEquatable<TKey>
{

    #region fields

    /// <summary>
    /// The application database context.
    /// </summary>
    private ILgApplicationDbContext _db;

    /// <summary>
    /// A SignalR Hub.
    /// </summary>
    private IHubContext<NotificationHub> _hub;

    /// <summary>
    /// The method tu use for convert a TKey to a Guid.
    /// </summary>
    private Func<Guid, TKey> _convert;

    #endregion

    #region constructors

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="db">The application database context.</param>
    /// <param name="hub">A SignalR Hub.</param>
    /// <param name="convert">The method tu use for convert a TKey to a Guid.</param>
    public LgNotificationManagerBase(ILgApplicationDbContext db, IHubContext<NotificationHub> hub, Func<Guid, TKey> convert)
    {
        _db = db;
        _hub = hub;
        _convert = convert;
    }

    #endregion

    #region Setters

    /// <summary>
    /// Save a notification
    /// </summary>
    /// <param name="notification">notification </param>
    /// <returns></returns>
    internal async Task SaveNotificationAsync(TNotification notification)
    {
        notification.CreationDate = DateTime.Now;
        await _db.Set<TNotification>().AddAsync(notification);
    }

    /// <summary>
    /// Save a notification user 
    /// </summary>
    /// <param name="notificationId">Notification Id</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    internal async Task SaveNotificationUserAsync(Guid notificationId, Guid userId)
    {
        TNotificationWithUser notificationUser = new()
        {
            NotificationId = notificationId,
            UserId = _convert(userId),
            UpdateDate = DateTime.Now
        };
        await _db.Set<TNotificationWithUser>().AddAsync(notificationUser);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Send a notification to a user
    /// </summary>
    /// <param name="notification">Notification object</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    public async Task SendNotificationAsync(TNotification notification, Guid userId)
    {
        await SaveNotificationAsync(notification);
        await SaveNotificationUserAsync(notification.Id, userId);
        await _db.SaveChangesAsync();
        await _hub.Clients.User(userId.ToString()).SendAsync("OnRefreshNotification");
    }

    /// <summary>
    ///  Send a notification to a list of users
    /// </summary>
    /// <param name="notification">Notification</param>
    /// <param name="userIds">User Id list</param>
    /// <returns></returns>
    public async Task SendNotificationAsync(TNotification notification, IEnumerable<Guid> userIds)
    {
        await SaveNotificationAsync(notification);
        foreach (Guid userId in userIds)
        {
            await SaveNotificationUserAsync(notification.Id, userId);
            await _hub.Clients.User(userId.ToString()).SendAsync("OnRefreshNotification");
        }
    }

    /// <summary>
    /// Update read indicator for a notification user
    /// </summary>
    /// <param name="notificationUserId">Notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="updateDate">Notification update date</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    public async Task UpdateNotificationReadStateAsync(Guid notificationUserId, bool isRead, Guid userId, DateTime? updateDate)
    {
        TNotificationWithUser notificatrionUser = await GetNotificationAsync(notificationUserId);
        if (notificatrionUser != null)
        {
            notificatrionUser.IsRead = isRead;
            notificatrionUser.UpdateDate = ((DateTime)(updateDate != null ? updateDate : DateTime.Now));
            await _db.SaveChangesAsync();
            await _hub.Clients.User(userId.ToString()).SendAsync($"OnRefreshNotification");
        }
        else
        {
            // Notification has been removed from database => remove from indexeddb
            await _hub.Clients.User(userId.ToString()).SendAsync($"OnRemoveNotification", notificationUserId);
        }
    }

    /// <summary>
    /// Update read indicator for a notification user
    /// </summary>
    /// <param name="notificationUserId">Notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    public Task UpdateNotificationReadStateAsync(Guid notificationUserId, bool isRead, Guid userId)
    {
        return UpdateNotificationReadStateAsync(notificationUserId, isRead, userId, null);
    }

    /// <summary>
    /// Update read indicator for a notification user
    /// </summary>
    /// <param name="notificationUserIds">List of notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="updateDate">Notification update date</param>
    /// <returns></returns>
    public async Task UpdateNotificationReadStateAsync(List<Guid> notificationUserIds, bool isRead, DateTime? updateDate)
    {
        List<TNotificationWithUser> notificatrionUsers = await GetNotificationAsync(notificationUserIds);
        List<string> userIds = new();
        string userId;
        foreach (TNotificationWithUser notificatrionUser in notificatrionUsers)
        {
            userId = notificatrionUser.UserId.ToString();
            if (userIds is null || !userIds.Contains(userId))
            {
                userIds.Add(userId);
            }
            notificatrionUser.IsRead = isRead;
            notificatrionUser.UpdateDate = (DateTime)(updateDate != null ? updateDate : DateTime.Now);
        }
        await _db.SaveChangesAsync();
        // Send refresh notification list for earch concerned users
        foreach (string notificatrionUserId in userIds)
        {
            await _hub.Clients.User(notificatrionUserId).SendAsync($"OnRefreshNotification");
        }
    }

    /// <summary>
    /// Synchronize notificaton with pending action received
    /// </summary>
    /// <param name="pendingAction">pending action</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    async Task ILgNotificationManager<TVmNotification, TNotification>.SyncNotificationsFromPendingActions(NotificationPendingActionVm pendingAction, Guid userId)
    {
        switch (pendingAction.Action)
        {
            case NotificationPendingAction.Update:
                await UpdateNotificationReadStateAsync(pendingAction.NotificationUserId, pendingAction.IsRead, userId, pendingAction.UpdateDate);
                break;
            case NotificationPendingAction.Delete:
                await DeleteUserNotificationAsync(pendingAction.NotificationUserId, userId);
                break;
        }
    }

    #endregion

    #region Delete

    /// <summary>
    /// Delete a notification for a user
    /// </summary>
    /// <param name="notificationUserId">Notificaton User Id</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    public async Task DeleteUserNotificationAsync(Guid notificationUserId, Guid userId)
    {
        TNotificationWithUser notificationUser = await GetNotificationAsync(notificationUserId);
        // notification == null notification could be deleted out of app context (ex : external batch)
        if (notificationUser != null)
        {
            Guid notificationId = notificationUser.NotificationId;
            _db.Set<TNotificationWithUser>().Remove(notificationUser);
            await _db.SaveChangesAsync();
            // Check if some notification users are already linked to the notification, otherwise remove noitifcation from database
            await DeleteNotificationUnassignedAsync(notificationId, true);
        }
        await _hub.Clients.User(userId.ToString()).SendAsync($"OnRemoveNotification", notificationUserId);
    }

    /// <summary>
    /// Delete a list of notification user
    /// </summary>
    /// <param name="notificationUserIds">list of notificaton User Id</param>
    /// <returns></returns>
    public async Task DeleteUserNotificationAsync(List<Guid> notificationUserIds)
    {
        List<TNotificationWithUser> notificationUsers = await GetNotificationAsync(notificationUserIds);
        Dictionary<string, List<Guid>> userIds = new();

        foreach (TKey userId in notificationUsers.Select(x => x.UserId).Distinct())
        {
            userIds.Add(userId.ToString(), notificationUsers
                .Where(x => x.UserId.Equals(userId)).Select(x => x.Id).ToList());
        }
        if (notificationUsers != null)
        {
            // Remove list from database
            _db.Set<TNotificationWithUser>().RemoveRange(notificationUsers);
            await _db.SaveChangesAsync();
            // Check if some notification users are already linked to the notification, otherwise remove noitifcation from database
            await DeleteNotificationUnassignedAsync(notificationUsers.Select(x => x.NotificationId).Distinct().ToList());
        }
        // Send refresh notification list for earch concerned users
        foreach (KeyValuePair<string, List<Guid>> entry in userIds)
        {
            await _hub.Clients.User(entry.Key).SendAsync($"OnRemoveNotifications", entry.Value);
        }
    }

    /// <summary>
    /// Remove unassigned notification one or many users
    /// Check if some notification users are already linked to the notification, otherwise remove noitifcation from database
    /// </summary>
    /// <param name="notificationId">notification id</param>
    /// <param name="saveChangesAsync">Save must be done</param>
    public async Task DeleteNotificationUnassignedAsync(Guid notificationId, bool saveChangesAsync = false)
    {
        if (!await _db.Set<TNotificationWithUser>().Where(x => x.NotificationId == notificationId).AnyAsync())
        {
            _db.Set<TNotification>().Remove(new TNotification() { Id = notificationId });
            if (saveChangesAsync)
            {
                await _db.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Remove unassigned notification to one or many users
    /// Check if some notification users are already linked to the notification, otherwise remove noitifcation from database
    /// </summary>
    /// <param name="notificationIds">List ofnotification id</param>
    public async Task DeleteNotificationUnassignedAsync(List<Guid> notificationIds)
    {
        foreach (Guid notificationId in notificationIds)
        {
            await DeleteNotificationUnassignedAsync(notificationId);
        }
        await _db.SaveChangesAsync();
    }

    #endregion

    #region Getters

    /// <summary>
    /// Retrieve notificaton from user notification id
    /// </summary>
    /// <param name="userNotificationId">User Notification Id</param>
    /// <returns></returns>
    public Task<TNotificationWithUser> GetNotificationAsync(Guid userNotificationId)
    {
        return _db.Set<TNotificationWithUser>().Where(x => x.Id == userNotificationId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieve notificatons from a list of user notification id
    /// </summary>
    /// <param name="userNotificationIds">List of User Notification Id</param>
    /// <returns></returns>
    public Task<List<TNotificationWithUser>> GetNotificationAsync(List<Guid> userNotificationIds)
    {
        return _db.Set<TNotificationWithUser>().Where(x => userNotificationIds.Contains(x.Id)).ToListAsync();
    }

    /// <summary>
    /// Retrieve a list of notification for one user
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    public async Task<List<TVmNotification>> GetNotificationsAsync(Guid userId)
    {
        TKey uid = _convert(userId);
        List<TVmNotification> notifications = new();
        List<TNotificationWithUser> userNotifications = await _db.Set<TNotificationWithUser>()
            .Where(x => x.UserId.Equals(uid))
            .Include(x => x.Notification)
            .ToListAsync();
        foreach (TNotificationWithUser userNotification in userNotifications)
        {

            notifications.Add(GetMappedVmNotification(userNotification));
        }
        return notifications;
    }

    /// <summary>
    /// Get notifications updated / added after the last notification update date
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="lastNotificationUpdt">last notification update date</param>
    /// <returns></returns>
    public async Task<List<TVmNotification>> GetNotificationsAsync(Guid userId, DateTime lastNotificationUpdt)
    {
        TKey uid = _convert(userId);
        List<TVmNotification> notifications = new();
        List<TNotificationWithUser> userNotifications = await _db.Set<TNotificationWithUser>()
            .Include(x => x.Notification)
            .Where(x => x.UserId.Equals(uid))
            .Where(x => x.UpdateDate.Date.CompareTo(lastNotificationUpdt.Date) >= 0)
            .ToListAsync();
        foreach (TNotificationWithUser userNotification in userNotifications)
        {

            notifications.Add(GetMappedVmNotification(userNotification));
        }
        return notifications;
    }

    /// <summary>
    /// Create the view model class initialized with the model values.
    /// </summary>
    /// <param name="userNotification">User's notification model.</param>
    /// <returns>The view model class initialized with the model values.</returns>
    private TVmNotification GetMappedVmNotification(TNotificationWithUser userNotification)
    {
        TVmNotification vmNotification = new()
        {
            Id = userNotification.NotificationId,
            NotificationUserId = userNotification.Id,
            IsRead = userNotification.IsRead,
            UpdateDate = userNotification.UpdateDate,
            Title = userNotification.Notification.Title,
            Description = userNotification.Notification.Description,
            CreationDate = userNotification.Notification.CreationDate
        };
        MapViewModel(vmNotification, userNotification.Notification);
        return vmNotification;
    }

    /// <summary>
    /// Method to map custom notification model fields to the view model class.
    /// </summary>
    /// <param name="vmNotification">The view model class pre-initialized.</param>
    /// <param name="notification">The notification informations.</param>
    protected abstract void MapViewModel(TVmNotification vmNotification, TNotification notification);

    #endregion

}
