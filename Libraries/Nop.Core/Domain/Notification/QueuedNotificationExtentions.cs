using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Notification
{
    public static class QueuedNotificationExtentions
    {
        public static void AddObserver(this QueuedNotification queuedNotification, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
            {
                if (!queuedNotification.ListOfObserverIdentifier.Any(e => e == identifier))
                {
                    List<string> identifiers = queuedNotification.ListOfObserverIdentifier.ToList();
                    identifiers.Add(identifier);
                    queuedNotification.ObserverIdentifier = string.Join(",", identifiers);
                }
            }
        }

        public static bool IsCheckMessage(this QueuedNotification queuedNotification, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
            {
                return queuedNotification.ListOfObserverIdentifier.Any(e => e == identifier);
            }

            throw new ArgumentNullException(identifier);
        }
    }
}
