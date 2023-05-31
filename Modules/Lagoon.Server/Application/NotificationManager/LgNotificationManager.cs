using Lagoon.Hubs;
using Lagoon.Model.Context;
using Lagoon.Model.Models;
using Microsoft.AspNetCore.SignalR;

namespace Lagoon.Server.Application;


/// <summary>
/// Service used to manage eula data
/// </summary>
/// <typeparam name="TUser">Because of the dbContext we must have the IdentityUser type used by the application</typeparam>
/// <typeparam name="TVmNotification">User notification view model</typeparam>
/// <typeparam name="TNotification">Notification model</typeparam>
public abstract class LgNotificationManager<TUser, TVmNotification, TNotification>
    : LgNotificationManagerBase<TUser, TVmNotification, TNotification, NotificationUserWithUser<TNotification, TUser>, string>
    where TUser : class, ILgIdentityUser
    where TVmNotification : NotificationVmBase, new()
    where TNotification : NotificationBase, new()
{
    #region constructors

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="db"> Application db context</param>
    /// <param name="hub">Hub (signalr)</param>
    public LgNotificationManager(ILgApplicationDbContext db, IHubContext<NotificationHub> hub)
        : base(db, hub, g => g.ToString())
    { }

    #endregion

}
