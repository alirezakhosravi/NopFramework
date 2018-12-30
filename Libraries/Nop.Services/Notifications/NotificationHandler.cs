using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Notification;
using Nop.Core.Events;

namespace Nop.Services.Notifications
{
    public partial class NotificationHandler : INotificationHandler
    {
        #region properties
        private List<INotificationObserver> _observers;
        private IList<INotificationObserver> Observers
        {
            get
            {
                if (_observers == null)
                {
                    _observers =
                        new List<INotificationObserver>();
                }

                return _observers;
            }
        }
        #endregion

        #region utility
        private QueuedNotification CastToMessageTemplate<T>(T message)
        {
            if(message is QueuedNotification)
            {
                return message as QueuedNotification;
            }

            if(message is EntityInsertedEvent<QueuedNotification>)
            {
                return (message as EntityInsertedEvent<QueuedNotification>).Entity;
            }

            if (message is EntityUpdatedEvent<QueuedNotification>)
            {
                return (message as EntityUpdatedEvent<QueuedNotification>).Entity;
            }

            return null;
        }
        #endregion

        #region ctor
        public NotificationHandler()
        {
        }
        #endregion

        public virtual void Register(INotificationObserver observer)
        {
            if (observer != null)
            {
                if (!Observers.Contains(observer))
                {
                    Observers.Add(observer);
                }
            }
        }

        public virtual void Unregister(INotificationObserver observer)
        {
            if (observer != null)
            {
                if (Observers.Contains(observer))
                {
                    Observers.Remove(observer);
                }
            }
        }

        public virtual void NotifyObservers<T>(T message)
        {
            QueuedNotification template = CastToMessageTemplate(message);

            if (template == null)
            {
                return;
            }

            Observers.ToList().ForEach(e => e.Notify(template));
        }
    }
}
