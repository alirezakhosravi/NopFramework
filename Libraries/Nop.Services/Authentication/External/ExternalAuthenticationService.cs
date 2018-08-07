using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Localization;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents external authentication service implementation
    /// </summary>
    public partial class ExternalAuthenticationService : IExternalAuthenticationService
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ExternalAuthenticationService(UserSettings userSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationService authenticationService,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPluginFinder pluginFinder,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            this._userSettings = userSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._authenticationService = authenticationService;
            this._userRegistrationService = userRegistrationService;
            this._userService = userService;
            this._eventPublisher = eventPublisher;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authenticate user with existing associated external account
        /// </summary>
        /// <param name="associatedUser">Associated with passed external authentication parameters user</param>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult AuthenticateExistingUser(User associatedUser, User currentLoggedInUser, string returnUrl)
        {
            //log in guest user
            if (currentLoggedInUser == null)
                return LoginUser(associatedUser, returnUrl);

            //account is already assigned to another user
            if (currentLoggedInUser.Id != associatedUser.Id)
                //TODO create locale for error
                return ErrorAuthentication(new[] { "Account is already assigned" }, returnUrl);

            //or the user try to log in as himself. bit weird
            return SuccessfulAuthentication(returnUrl);
        }

        /// <summary>
        /// Authenticate current user and associate new external account with user
        /// </summary>
        /// <param name="currentLoggedInUser">Current logged-in user</param>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult AuthenticateNewUser(User currentLoggedInUser, ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //associate external account with logged-in user
            if (currentLoggedInUser != null)
            {
                AssociateExternalAccountWithUser(currentLoggedInUser, parameters);
                return SuccessfulAuthentication(returnUrl);
            }

            //or try to register new user
            if (_userSettings.UserRegistrationType != UserRegistrationType.Disabled)
                return RegisterNewUser(parameters, returnUrl);

            //registration is disabled
            return ErrorAuthentication(new[] { "Registration is disabled" }, returnUrl);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="parameters">Authentication parameters received from external authentication method</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult RegisterNewUser(ExternalAuthenticationParameters parameters, string returnUrl)
        {
            //check whether the specified email has been already registered
            if (_userService.GetUserByEmail(parameters.Email) != null)
            {
                var alreadyExistsError = string.Format(_localizationService.GetResource("Account.AssociatedExternalAuth.EmailAlreadyExists"),
                    !string.IsNullOrEmpty(parameters.ExternalDisplayIdentifier) ? parameters.ExternalDisplayIdentifier : parameters.ExternalIdentifier);
                return ErrorAuthentication(new[] { alreadyExistsError }, returnUrl);
            }

            //registration is approved if validation isn't required
            var registrationIsApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard ||
                (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation && !_externalAuthenticationSettings.RequireEmailValidation);

            //create registration request
            var registrationRequest = new UserRegistrationRequest(_workContext.CurrentUser,
                parameters.Email, parameters.Email,
                CommonHelper.GenerateRandomDigitCode(20),
                PasswordFormat.Clear,
                registrationIsApproved);

            //whether registration request has been completed successfully
            var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
            if (!registrationResult.Success)
                return ErrorAuthentication(registrationResult.Errors, returnUrl);

            //allow to save other user values by consuming this event
            _eventPublisher.Publish(new UserAutoRegisteredByExternalMethodEvent(_workContext.CurrentUser, parameters));

            //raise user registered event
            _eventPublisher.Publish(new UserRegisteredEvent(_workContext.CurrentUser));

            //associate external account with registered user
            AssociateExternalAccountWithUser(_workContext.CurrentUser, parameters);

            //authenticate
            if (registrationIsApproved)
            {
                _authenticationService.SignIn(_workContext.CurrentUser, false);
                _workflowMessageService.SendUserWelcomeMessage(_workContext.CurrentUser, _workContext.WorkingLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
            }

            //registration is succeeded but isn't activated
            if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            {
                //email validation message
                _genericAttributeService.SaveAttribute(_workContext.CurrentUser, NopUserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                _workflowMessageService.SendUserEmailValidationMessage(_workContext.CurrentUser, _workContext.WorkingLanguage.Id);

                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
            }

            //registration is succeeded but isn't approved by admin
            if (_userSettings.UserRegistrationType == UserRegistrationType.AdminApproval)
                return new RedirectToRouteResult("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });

            return ErrorAuthentication(new[] { "Error on registration" }, returnUrl);
        }

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="user">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult LoginUser(User user, string returnUrl)
        {
            //authenticate
            _authenticationService.SignIn(user, false);

            //raise event       
            _eventPublisher.Publish(new UserLoggedinEvent(user));

            return SuccessfulAuthentication(returnUrl);
        }

        /// <summary>
        /// Add errors that occurred during authentication
        /// </summary>
        /// <param name="errors">Collection of errors</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult ErrorAuthentication(IEnumerable<string> errors, string returnUrl)
        {
            foreach (var error in errors)
                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

            return new RedirectToActionResult("Login", "User", !string.IsNullOrEmpty(returnUrl) ? new { ReturnUrl = returnUrl } : null);
        }

        /// <summary>
        /// Redirect the user after successful authentication
        /// </summary>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected virtual IActionResult SuccessfulAuthentication(string returnUrl)
        {
            //redirect to the return URL if it's specified
            if (!string.IsNullOrEmpty(returnUrl))
                return new RedirectResult(returnUrl);

            return new RedirectToRouteResult("HomePage", null);
        }

        #endregion

        #region Methods

        #region External authentication methods

        /// <summary>
        /// Load active external authentication methods
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <returns>Payment methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods(User user = null)
        {
            return LoadAllExternalAuthenticationMethods(user)
                .Where(provider => _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames
                    .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        public virtual IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExternalAuthenticationMethod>(systemName);
            return descriptor?.Instance<IExternalAuthenticationMethod>();
        }

        /// <summary>
        /// Load all external authentication methods
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <returns>External authentication methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods(User user = null)
        {
            return _pluginFinder.GetPlugins<IExternalAuthenticationMethod>(user: user).ToList();
        }

        /// <summary>
        /// Check whether authentication by the passed external authentication method is available
        /// </summary>
        /// <param name="systemName">System name of the external authentication method</param>
        /// <returns>True if authentication is available; otherwise false</returns>
        public virtual bool ExternalAuthenticationMethodIsAvailable(string systemName)
        {
            //load method
            var authenticationMethod = LoadExternalAuthenticationMethodBySystemName(systemName);

            return authenticationMethod != null &&
                this.IsExternalAuthenticationMethodActive(authenticationMethod) &&
                authenticationMethod.PluginDescriptor.Installed &&
                _pluginFinder.AuthorizedForUser(authenticationMethod.PluginDescriptor, _workContext.CurrentUser);
        }

        /// <summary>
        /// Check whether external authentication method is active
        /// </summary>
        /// <param name="method">External authentication method</param>
        /// <returns>True if method is active; otherwise false</returns>
        public virtual bool IsExternalAuthenticationMethodActive(IExternalAuthenticationMethod method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames == null)
                return false;

            foreach (var activeMethodSystemName in _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames)
                if (method.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
        #endregion

        #region Authentication

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        public virtual IActionResult Authenticate(ExternalAuthenticationParameters parameters, string returnUrl = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (!ExternalAuthenticationMethodIsAvailable(parameters.ProviderSystemName))
                return ErrorAuthentication(new[] { "External authentication method cannot be loaded" }, returnUrl);

            //get current logged-in user
            var currentLoggedInUser = _workContext.CurrentUser.IsRegistered() ? _workContext.CurrentUser : null;

            //authenticate associated user if already exists
            var associatedUser = GetUserByExternalAuthenticationParameters(parameters);
            if (associatedUser != null)
                return AuthenticateExistingUser(associatedUser, currentLoggedInUser, returnUrl);

            //or associate and authenticate new user
            return AuthenticateNewUser(currentLoggedInUser, parameters, returnUrl);
        }

        #endregion

        /// <summary>
        /// Associate external account with user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">External authentication parameters</param>
        public virtual void AssociateExternalAccountWithUser(User user, ExternalAuthenticationParameters parameters)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                UserId = user.Id,
                Email = parameters.Email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthAccessToken = parameters.AccessToken,
                ProviderSystemName = parameters.ProviderSystemName
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>User</returns>
        public virtual User GetUserByExternalAuthenticationParameters(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _externalAuthenticationRecordRepository.Table.FirstOrDefault(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
            if (associationRecord == null)
                return null;

            return _userService.GetUserById(associationRecord.UserId);
        }

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        public virtual void RemoveAssociation(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _externalAuthenticationRecordRepository.Table.FirstOrDefault(record =>
                record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));

            if (associationRecord != null)
                _externalAuthenticationRecordRepository.Delete(associationRecord);
        }

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        public virtual void DeleteExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            if (externalAuthenticationRecord == null)
                throw new ArgumentNullException(nameof(externalAuthenticationRecord));

            _externalAuthenticationRecordRepository.Delete(externalAuthenticationRecord);
        }

        #endregion
    }
}