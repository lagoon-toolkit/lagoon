//TODEL
//using System;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Lagoon.Model.Models
//{
//    /// <summary>
//    /// Notification user data
//    /// </summary>
//    /// <typeparam name="TNotification"></typeparam>
//    public abstract class NotificationUserBase<TNotification>
//        where TNotification: NotificationBase
//    {

//        #region properties

//        /// <summary>
//        /// Gets or sets the notification id.
//        /// </summary>
//        public Guid Id { get; set; } = Guid.NewGuid();

//        /// <summary>
//        /// Gets or sets the notification id.
//        /// </summary>
//        [ForeignKey(nameof(Notification))]
//        public Guid NotificationId { get; set; }

//        /// <summary>
//        /// Notification object
//        /// </summary>
//        public virtual TNotification Notification { get; set; }

//        /// <summary>
//        /// Gets or sets if the notification is read.
//        /// </summary>
//        public bool IsRead { get; set; }

//        /// <summary>
//        /// Gets or sets the notification update date.
//        /// </summary>
//        public DateTime UpdateDate { get; set; }

//        /// <summary>
//        /// Gets or sets the user id.
//        /// </summary>
//        public abstract string UserId { get; set; }

//        #endregion

//    }
//}
