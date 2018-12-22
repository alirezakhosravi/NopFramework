using System;
using System.Collections.Generic;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;

namespace Nop.Services.Notifications
{
    public interface IWorkflowNotificationService
    {
        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="userIds">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendTestNotification(int messageTemplateId, string userIds, List<Token> tokens, int languageId);

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <returns>The notification.</returns>
        /// <param name="userIds">User identifiers.</param>
        /// <param name="messageTemplate">Message template.</param>
        /// <param name="languageId">Language identifier.</param>
        /// <param name="tokens">Tokens.</param>
        /// <param name="attachmentFilePath">Attachment file path.</param>
        /// <param name="attachmentFileName">Attachment file name.</param>
        /// <param name="subject">Subject.</param>
        int SendNotification(string userIds, MessageTemplate messageTemplate,
            int languageId, IEnumerable<Token> tokens,
            string attachmentFilePath = null, string attachmentFileName = null, string subject = null);

        int SendNotification(string userIds, string subject, string body,
                             string attachmentFilePath = null, string attachmentFileName = null);
    }
}
