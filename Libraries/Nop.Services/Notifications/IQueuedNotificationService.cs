using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Notification;

namespace Nop.Services.Notifications
{
    /// <summary>
    /// Queued notification service.
    /// </summary>
    public partial interface IQueuedNotificationService
    {
        /// <summary>
        /// Inserts a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>
        void InsertQueuedNotification(QueuedNotification queuedNotification);

        /// <summary>
        /// Updates a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>
        void UpdateQueuedNotification(QueuedNotification queuedNotification);

        /// <summary>
        /// Deleted a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>
        void DeleteQueuedNotification(QueuedNotification queuedNotification);

        /// <summary>
        /// Deleted a queued notifications
        /// </summary>
        /// <param name="queuedNotifications">Queued notifications</param>
        void DeleteQueuedNotifications(IList<QueuedNotification> queuedNotifications);

        /// <summary>
        /// Gets a queued notification by identifier
        /// </summary>
        /// <param name="queuedNotificationId">Queued notification identifier</param>
        /// <returns>Queued notification</returns>
        QueuedNotification GetQueuedNotificationById(int queuedNotificationId);

        /// <summary>
        /// Get queued notifications by identifiers
        /// </summary>
        /// <param name="queuedNotificationIds">queued notification identifiers</param>
        /// <returns>Queued notifications</returns>
        IList<QueuedNotification> GetQueuedNotificationsByIds(int[] queuedNotificationIds);

        /// <summary>
        /// Search queued notifications
        /// </summary>
        /// <param name="fromNotification">From Notification</param>
        /// <param name="toNotification">To Notification</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent notifications</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only notifications for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued notification descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Queued notifications</returns>
        IPagedList<QueuedNotification> SearchNotifications(string userIds,
            DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Delete all queued notifications
        /// </summary>
        void DeleteAllNotifications();
    }
}
