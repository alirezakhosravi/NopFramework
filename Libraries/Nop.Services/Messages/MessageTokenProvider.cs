using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly SiteInformationSettings _siteInformationSettings;

        private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public MessageTokenProvider(IActionContextAccessor actionContextAccessor,
            IAddressAttributeFormatter addressAttributeFormatter,
            IUserAttributeFormatter userAttributeFormatter,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            SiteInformationSettings siteInformationSettings)
        {
            this._actionContextAccessor = actionContextAccessor;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._userAttributeFormatter = userAttributeFormatter;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._downloadService = downloadService;
            this._eventPublisher = eventPublisher;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._urlHelperFactory = urlHelperFactory;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._templatesSettings = templatesSettings;
            this._siteInformationSettings = siteInformationSettings;
        }

        #endregion

        #region Allowed tokens

        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //user tokens
                _allowedTokens.Add(TokenGroupNames.UserTokens, new[]
                {
                    "%User.Email%",
                    "%User.Username%",
                    "%User.FullName%",
                    "%User.FirstName%",
                    "%User.LastName%",
                    "%User.VatNumber%",
                    "%User.VatNumberStatus%",
                    "%User.CustomAttributes%",
                    "%User.PasswordRecoveryURL%",
                    "%User.AccountActivationURL%",
                    "%User.EmailRevalidationURL%",
                    "%Wishlist.URLForUser%"
                });

                //recurring payment tokens
                _allowedTokens.Add(TokenGroupNames.RecurringPaymentTokens, new[]
                {
                    "%RecurringPayment.ID%",
                    "%RecurringPayment.CancelAfterFailedPayment%",
                    "%RecurringPayment.RecurringPaymentType%"
                });

                //newsletter subscription tokens
                _allowedTokens.Add(TokenGroupNames.SubscriptionTokens, new[]
                {
                    "%NewsLetterSubscription.Email%",
                    "%NewsLetterSubscription.ActivationUrl%",
                    "%NewsLetterSubscription.DeactivationUrl%"
                });

                //private message tokens
                _allowedTokens.Add(TokenGroupNames.PrivateMessageTokens, new[]
                {
                    "%PrivateMessage.Subject%",
                    "%PrivateMessage.Text%"
                });

                //contact us tokens
                _allowedTokens.Add(TokenGroupNames.ContactUs, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                //contact vendor tokens
                _allowedTokens.Add(TokenGroupNames.ContactVendor, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                return _allowedTokens;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generates an absolute URL for the specified site, routeName and route values
        /// </summary>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>Generated URL</returns>
        protected virtual string RouteUrl(string routeName = null, object routeValues = null)
        {
            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = new PathString(urlHelper.RouteUrl(routeName, routeValues));

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{url}"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        public virtual void AddUserTokens(IList<Token> tokens, User user)
        {
            tokens.Add(new Token("User.Email", user.Email));
            tokens.Add(new Token("User.Username", user.Username));
            tokens.Add(new Token("User.FullName", _userService.GetUserFullName(user)));
            tokens.Add(new Token("User.FirstName", _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FirstNameAttribute)));
            tokens.Add(new Token("User.LastName", _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastNameAttribute)));
            tokens.Add(new Token("User.VatNumber", _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.VatNumberAttribute)));

            var customAttributesXml = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CustomUserAttributes);
            tokens.Add(new Token("User.CustomAttributes", _userAttributeFormatter.FormatAttributes(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = RouteUrl(routeName: "PasswordRecoveryConfirm", routeValues: new { token = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.PasswordRecoveryTokenAttribute), email = user.Email });
            var accountActivationUrl = RouteUrl(routeName: "AccountActivation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.AccountActivationTokenAttribute), email = user.Email });
            var emailRevalidationUrl = RouteUrl(routeName: "EmailRevalidation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.EmailRevalidationTokenAttribute), email = user.Email });
            var wishlistUrl = RouteUrl(routeName: "Wishlist", routeValues: new { userGuid = user.UserGuid });
            tokens.Add(new Token("User.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("User.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("User.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForUser", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(user, tokens);
        }

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        public virtual void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            var activationUrl = RouteUrl(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "true" });
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl, true));

            var deactivationUrl = RouteUrl(routeName: "NewsletterActivation", routeValues: new { token = subscription.NewsLetterSubscriptionGuid, active = "false" });
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deactivationUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(subscription, tokens);
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>Collection of allowed (supported) message tokens for campaigns</returns>
        public virtual IEnumerable<string> GetListOfCampaignAllowedTokens()
        {
            var additionTokens = new CampaignAdditionTokensAddedEvent();
            _eventPublisher.Publish(additionTokens);

            var allowedTokens = GetListOfAllowedTokens(new[] { TokenGroupNames.SiteTokens, TokenGroupNames.SubscriptionTokens }).ToList();
            allowedTokens.AddRange(additionTokens.AdditionTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        public virtual IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null)
        {
            var additionTokens = new AdditionTokensAddedEvent();
            _eventPublisher.Publish(additionTokens);

            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            allowedTokens.AddRange(additionTokens.AdditionTokens);

            return allowedTokens.Distinct();
        }

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            switch (messageTemplate.Name)
            {
                case MessageTemplateSystemNames.UserRegisteredNotification:
                case MessageTemplateSystemNames.UserWelcomeMessage:
                case MessageTemplateSystemNames.UserEmailValidationMessage:
                case MessageTemplateSystemNames.UserEmailRevalidationMessage:
                case MessageTemplateSystemNames.UserPasswordRecoveryMessage:
                    return new[] { TokenGroupNames.SiteTokens, TokenGroupNames.UserTokens };

                case MessageTemplateSystemNames.ContactUsMessage:
                    return new[] { TokenGroupNames.SiteTokens, TokenGroupNames.ContactUs };

                default:
                    return new string[] { };
            }
        }

        #endregion
    }
}