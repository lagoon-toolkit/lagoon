using Lagoon.UI.Components;
using System;

namespace Demo.Shared.ViewModels;

/// <summary>
/// Notification view model
/// </summary>
public class NotificationVm : NotificationVmBase
{
    /// <summary>
    /// Constructor
    /// </summary>
    public NotificationVm()
    {

    }

    /// <summary>
    /// Compare notification
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override int CompareTo(NotificationVmBase other)
    {
        return base.CompareTo(other);
    }
}
