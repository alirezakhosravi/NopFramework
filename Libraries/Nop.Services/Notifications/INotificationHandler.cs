using System.Collections.Generic;

namespace Nop.Services.Notifications
{
    public partial interface INotificationHandler
    {
        /// <summary>
        /// Attach
        /// </summary>
        void Register(INotificationObserver observer);

        /// <summary>
        /// Detach
        /// </summary>
        void Unregister(INotificationObserver observer);

        /// <summary>
        /// Notify
        /// </summary>
        void NotifyObservers<T>(T message);
    }
}
