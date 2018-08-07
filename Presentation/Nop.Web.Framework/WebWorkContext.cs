using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Tasks;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly LocalizationSettings _localizationSettings;

        private User _cachedUser;
        private User _originalUserIfImpersonated;
        private Language _cachedLanguage;

        #endregion

        #region Ctor

        public WebWorkContext(IAuthenticationService authenticationService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IUserAgentHelper userAgentHelper,
            LocalizationSettings localizationSettings)
        {
            this._authenticationService = authenticationService;
            this._userService = userService;
            this._genericAttributeService = genericAttributeService;
            this._httpContextAccessor = httpContextAccessor;
            this._languageService = languageService;
            this._userAgentHelper = userAgentHelper;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get nop user cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetUserCookie()
        {
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.UserCookie}";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set nop user cookie
        /// </summary>
        /// <param name="userGuid">Guid of the user</param>
        protected virtual void SetUserCookie(Guid userGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.UserCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (userGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, userGuid.ToString(), options);
        }

        /// <summary>
        /// Get language from the requested page URL
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //whether the requsted URL is localized
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(_httpContextAccessor.HttpContext.Request.PathBase, false, out Language language))
                return null;
            
            return language;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            //get request culture
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            //try to get language by culture name
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language =>
                language.LanguageCulture.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published)
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public virtual User CurrentUser
        {
            get
            {
                //whether there is a cached value
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;

                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext == null ||
                    _httpContextAccessor.HttpContext.Request.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase))
                {
                    //in this case return built-in user record for background task
                    user = _userService.GetUserBySystemName(NopUserDefaults.BackgroundTaskUserName);
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in user record for search engines
                    if (_userAgentHelper.IsSearchEngine())
                        user = _userService.GetUserBySystemName(NopUserDefaults.SearchEngineUserName);
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //try to get registered user
                    user = _authenticationService.GetAuthenticatedUser();
                }

                if (user != null && !user.Deleted && user.Active && !user.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedUserId = _genericAttributeService
                        .GetAttribute<int?>(user, NopUserDefaults.ImpersonatedUserIdAttribute);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted && impersonatedUser.Active && !impersonatedUser.RequireReLogin)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //get guest user
                    var userCookie = GetUserCookie();
                    if (!string.IsNullOrEmpty(userCookie))
                    {
                        if (Guid.TryParse(userCookie, out Guid userGuid))
                        {
                            //get user from cookie (should not be registered)
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null && !userByCookie.IsRegistered())
                                user = userByCookie;
                        }
                    }
                }

                if (user == null || user.Deleted || !user.Active || user.RequireReLogin)
                {
                    //create guest if not exists
                    user = _userService.InsertGuestUser();
                }

                if (!user.Deleted && user.Active && !user.RequireReLogin)
                {
                    //set user cookie
                    SetUserCookie(user.UserGuid);

                    //cache the found user
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets the original user (in case the current one is impersonated)
        /// </summary>
        public virtual User OriginalUserIfImpersonated
        {
            get { return _originalUserIfImpersonated; }
        }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                //whether there is a cached value
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                Language detectedLanguage = null;

                //localized URLs are enabled, so try to get language from the requested page URL
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    detectedLanguage = GetLanguageFromUrl();

                //whether we should detect the language from the request
                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    //whether language already detected by this way
                    var alreadyDetected = _genericAttributeService.GetAttribute<bool>(this.CurrentUser,
                        NopUserDefaults.LanguageAutomaticallyDetectedAttribute);

                    //if not, try to get language from the request
                    if (!alreadyDetected)
                    {
                        detectedLanguage = GetLanguageFromRequest();
                        if (detectedLanguage != null)
                        {
                            //language already detected
                            _genericAttributeService.SaveAttribute(this.CurrentUser, NopUserDefaults.LanguageAutomaticallyDetectedAttribute, true);
                        }
                    }
                }
                
                //if the language is detected we need to save it
                if (detectedLanguage != null)
                {
                    //get current saved language identifier
                    var currentLanguageId = _genericAttributeService.GetAttribute<int>(this.CurrentUser, NopUserDefaults.LanguageIdAttribute);

                    //save the detected language identifier if it differs from the current one
                    if (detectedLanguage.Id != currentLanguageId)
                    {
                        _genericAttributeService.SaveAttribute(this.CurrentUser,
                            NopUserDefaults.LanguageIdAttribute, detectedLanguage.Id);
                    }
                }
                //get current user language identifier
                var userLanguageId = _genericAttributeService.GetAttribute<int>(this.CurrentUser,
                    NopUserDefaults.LanguageIdAttribute);

                var allStoreLanguages = _languageService.GetAllLanguages();

                //check user language availability
                var userLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == userLanguageId);
                if (userLanguage == null)
                {
                    //it not found, then try to get the default language for the current store (if specified)
                    userLanguage = allStoreLanguages.FirstOrDefault();
                }

                //if the default language for the current store not found, then try to get the first one
                if (userLanguage == null)
                    userLanguage = allStoreLanguages.FirstOrDefault();

                //if there are no languages for the current store try to get the first one regardless of the store
                if (userLanguage == null)
                userLanguage = _languageService.GetAllLanguages().FirstOrDefault();

                //cache the found language
                _cachedLanguage = userLanguage;

                return _cachedLanguage;
            }
            set
            {
                //get passed language identifier
                var languageId = value?.Id ?? 0;

                //and save it
                _genericAttributeService.SaveAttribute(this.CurrentUser,
                    NopUserDefaults.LanguageIdAttribute, languageId);

                //then reset the cached value
                _cachedLanguage = null;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}