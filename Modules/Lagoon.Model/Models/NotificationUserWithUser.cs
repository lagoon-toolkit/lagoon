namespace Lagoon.Model.Models;


/// <summary>
/// The notification data base class.
/// </summary>    
public class NotificationUserWithUser<TNotification, TUser> : NotificationUserWithUserBase<TNotification, TUser, string>
    where TUser : ILgIdentityUser
{ }