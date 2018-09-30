using Nop.Core.Domain.Notification;

namespace Nop.Services.Notifications
{
    public partial interface INotificationObserver
    {
        string Identifier { get; }

        INotificationHandler Handler { get; set; }

        /// <summary>
        /// Update
        /// </summary>
        void Notify(QueuedNotification message);
    }
}
