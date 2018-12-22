using System;
using System.Linq;

namespace Nop.Core.Domain.Notification
{
    public partial class QueuedNotification : BaseEntity
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int PriorityId { get; set; }

        /// <summary>
        /// Gets or sets the user identifiers.
        /// </summary>
        public string UserIds { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the attachment file path (full file path)
        /// </summary>
        public string AttachmentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.
        /// </summary>
        public string AttachmentFileName { get; set; }

        /// <summary>
        /// Gets or sets the download identifier of attached file
        /// </summary>
        public int AttachedDownloadId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time in UTC before which this email should not be sent
        /// </summary>
        public DateTime? DontSendBeforeDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the observer identifier.
        /// </summary>
        public string ObserverIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public QueuedNotificationPriority Priority
        {
            get => (QueuedNotificationPriority)this.PriorityId;
            set => this.PriorityId = (int)value;
        }

        /// <summary>
        /// Gets the list of user identifiers.
        /// </summary>
        public int[] ListOfUserIds => !string.IsNullOrEmpty(UserIds) ? UserIds.Split(',').Select(e => int.Parse(e)).ToArray() : new int[0];

        /// <summary>
        /// Gets the list of identifier.
        /// </summary>
        public string[] ListOfObserverIdentifier => !string.IsNullOrEmpty(ObserverIdentifier) ? ObserverIdentifier.Split(',').ToArray() : new string[0];
    }
}
