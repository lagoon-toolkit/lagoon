namespace Lagoon.Model.Models;


/// <summary>
/// The notification data base class.
/// </summary>    
public class NotificationUserWithUserGuid<TNotification, TUser> : NotificationUserWithUserBase<TNotification, TUser, Guid>
    where TUser : ILgIdentityUser
{ }