using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Notification;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Users;

namespace Nop.Services.Notifications
{
    public partial class WorkflowNotificationService : IWorkflowNotificationService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IUserService _userService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IQueuedNotificationService _queuedNotificationService;
        private readonly ITokenizer _tokenizer;

        #endregion

        #region Ctor

        public WorkflowNotificationService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IUserService userService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IQueuedEmailService queuedEmailService,
            IQueuedNotificationService queuedNotificationService,
            ITokenizer tokenizer)
        {
            this._commonSettings = commonSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._userService = userService;
            this._emailAccountService = emailAccountService;
            this._eventPublisher = eventPublisher;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._messageTemplateService = messageTemplateService;
            this._messageTokenProvider = messageTokenProvider;
            this._queuedEmailService = queuedEmailService;
            this._queuedNotificationService = queuedNotificationService;
            this._tokenizer = tokenizer;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>List of message templates</returns>
        protected virtual IList<MessageTemplate> GetActiveMessageTemplates(string messageTemplateName)
        {
            //get message templates by the name
            var messageTemplates = _messageTemplateService.GetMessageTemplatesByName(messageTemplateName);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Get EmailAccount to use with a message templates
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>EmailAccount</returns>
        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = _localizationService.GetLocalized(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (_emailAccountService.GetEmailAccountById(emailAccountId) ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)) ??
                               _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Ensure language is active
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Return a value language identifier</returns>
        protected virtual int EnsureLanguageIsActive(int languageId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        #endregion

        #region Methods

        #region Misc

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="userIds">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendTestNotification(int messageTemplateId, string userIds, List<Token> tokens, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(string.Empty, messageTemplate, languageId, tokens);
        }

        public virtual int SendNotification(string userIds, MessageTemplate messageTemplate,
            int languageId, IEnumerable<Token> tokens,
            string attachmentFilePath = null, string attachmentFileName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            //retrieve localized message template data
            if (string.IsNullOrEmpty(subject))
                subject = _localizationService.GetLocalized(messageTemplate, mt => mt.Subject, languageId);
            var body = _localizationService.GetLocalized(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            var notification = new QueuedNotification
            {
                Priority = QueuedNotificationPriority.High,
                UserIds = userIds,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedNotificationService.InsertQueuedNotification(notification);
            return notification.Id;
        }

        public virtual int SendNotification(string userIds, string subject, string body,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            MessageTemplate messageTemplate = _messageTemplateService.GetMessageTemplatesByName("").FirstOrDefault();

            var notification = new QueuedNotification
            {
                Priority = QueuedNotificationPriority.High,
                UserIds = userIds,
                Subject = subject,
                Body = body,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate?.AttachedDownloadId ?? 0,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = (!messageTemplate?.DelayBeforeSend.HasValue ?? true) ? null
                                         : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedNotificationService.InsertQueuedNotification(notification);
            return notification.Id;
        }

        #endregion

        #endregion
    }
}
