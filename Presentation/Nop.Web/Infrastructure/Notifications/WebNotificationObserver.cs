using System;
using System.Linq;
using Nop.Core.Domain.Notification;
using Nop.Services.Notifications;
using Nop.Web.Infrastructure.Hubs;
using Nop.Services.Users;

namespace Nop.Web.Infrastructure.Notifications
{
    public class WebNotificationObserver : INotificationObserver
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly INotificationHub _notificationHub;
        private readonly IQueuedNotificationService _queuedNotificationService;
        private readonly IUserService _userService;

        public WebNotificationObserver(
            INotificationHandler notificationHandler,
            INotificationHub notificationHub,
            IQueuedNotificationService queuedNotificationService,
            IUserService userService)
        {
            _notificationHandler = notificationHandler;
            _notificationHub = notificationHub;
            _queuedNotificationService = queuedNotificationService;
            _userService = userService;

            _notificationHandler.Register(this);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier => "WebNotificationObserver";

        /// <summary>
        /// Gets the handler.
        /// </summary>
        public INotificationHandler Handler
        {
            get => _notificationHandler;
            /// <summary>
            /// Sets the handler (NotImplemented).
            /// </summary>
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Notify the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        public void Notify(QueuedNotification message)
        {
            if (!message.IsCheckMessage(this.Identifier))
            {
                if (message.ListOfUserIds.Any())
                {
                    message.ListOfUserIds.ToList().ForEach(e => 
                        _notificationHub.SendNotificationToUser(_userService.GetUserById(e).Username, message.Body));
                }
                else
                {
                    _notificationHub.SendNotification(message.Body);
                }
                message.AddObserver(this.Identifier);
                _queuedNotificationService.UpdateQueuedNotification(message);
            }
        }
    }
}
