using Demo.Model.Models;
using Demo.Shared.ViewModels;
using Lagoon.Hubs;
using Lagoon.Server.Application;
using Microsoft.AspNetCore.SignalR;

namespace Demo.Server.Technical;

public class NotificationManager : LgNotificationManager<ApplicationUser, NotificationVm, Notification>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db"> Application db context</param>
    /// <param name="hub">Hub (signalr)</param>
    public NotificationManager(ApplicationDbContext db, IHubContext<NotificationHub> hub) : base(db, hub)
    {
    }

    protected override void MapViewModel(NotificationVm vmNotification, Notification notification)
    {
        throw new NotImplementedException();
    }
}
