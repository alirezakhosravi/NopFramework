using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class UserController : BaseAdminController
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IExportManager _exportManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public UserController(UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            IUserActivityService userActivityService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IUserModelFactory userModelFactory,
            IUserRegistrationService userRegistrationService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IExportManager exportManager,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
            this._userSettings = userSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._addressAttributeParser = addressAttributeParser;
            this._addressService = addressService;
            this._userActivityService = userActivityService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._userModelFactory = userModelFactory;
            this._userRegistrationService = userRegistrationService;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._exportManager = exportManager;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._permissionService = permissionService;
            this._queuedEmailService = queuedEmailService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        protected virtual string ValidateUserRoles(IList<UserRole> userRoles)
        {
            if (userRoles == null)
                throw new ArgumentNullException(nameof(userRoles));

            //ensure a user is not added to both 'Guests' and 'Registered' user roles
            //ensure that a user is in at least one required role ('Guests' and 'Registered')
            var isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == NopUserDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == NopUserDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return _localizationService.GetResource("Admin.Users.Users.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return _localizationService.GetResource("Admin.Users.Users.AddUserToGuestsOrRegisteredRoleError");

            //no errors
            return string.Empty;
        }

        protected virtual string ParseCustomUserAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                var controlId = $"user_attribute_{attribute.Id}";
            }

            return attributesXml;
        }

        private bool SecondAdminAccountExists(User user)
        {
            var users = _userService.GetAllUsers(userRoleIds: new[] { _userService.GetUserRoleBySystemName(NopUserDefaults.AdministratorsRoleName).Id });

            return users.Any(c => c.Active && c.Id != user.Id);
        }

        #endregion

        #region Users

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _userModelFactory.PrepareUserSearchModel(new UserSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult UserList(UserSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _userModelFactory.PrepareUserListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _userModelFactory.PrepareUserModel(new UserModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Create(UserModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!string.IsNullOrWhiteSpace(model.Email) && _userService.GetUserByEmail(model.Email) != null)
                ModelState.AddModelError(string.Empty, "Email is already registered");

            if (!string.IsNullOrWhiteSpace(model.Username) && _userSettings.UsernamesEnabled &&
                _userService.GetUserByUsername(model.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already registered");
            }

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!string.IsNullOrEmpty(userRolesError))
            {
                ModelState.AddModelError(string.Empty, userRolesError);
                ErrorNotification(userRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == NopUserDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"), false);
            }

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(model.Form);
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == NopUserDefaults.RegisteredRoleName) != null)
            {
                var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
                foreach (var error in userAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    AdminComment = model.AdminComment,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                _userService.InsertUser(user);

                //form fields
                if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                if (_userSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.GenderAttribute, model.Gender);
                _genericAttributeService.SaveAttribute(user, NopUserDefaults.FirstNameAttribute, model.FirstName);
                _genericAttributeService.SaveAttribute(user, NopUserDefaults.LastNameAttribute, model.LastName);
                if (_userSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.DateOfBirthAttribute, model.DateOfBirth);
                if (_userSettings.CompanyEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.CompanyAttribute, model.Company);
                if (_userSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.StreetAddressAttribute, model.StreetAddress);
                if (_userSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
                if (_userSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                if (_userSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.CityAttribute, model.City);
                if (_userSettings.CountyEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.CountyAttribute, model.County);
                if (_userSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.CountryIdAttribute, model.CountryId);
                if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                if (_userSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.PhoneAttribute, model.Phone);
                if (_userSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.FaxAttribute, model.Fax);

                //custom user attributes
                _genericAttributeService.SaveAttribute(user, NopUserDefaults.CustomUserAttributes, userAttributesXml);

                //password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                _userService.UpdateUser(user);

                //activity log
                _userActivityService.InsertActivity("AddNewUser",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewUser"), user.Id), user);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = user.Id });
            }

            //prepare model
            model = _userModelFactory.PrepareUserModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _userModelFactory.PrepareUserModel(null, user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(UserModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                return RedirectToAction("List");

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!string.IsNullOrEmpty(userRolesError))
            {
                ModelState.AddModelError(string.Empty, userRolesError);
                ErrorNotification(userRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == NopUserDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"), false);
            }

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(model.Form);
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == NopUserDefaults.RegisteredRoleName) != null)
            {
                var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
                foreach (var error in userAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.AdminComment = model.AdminComment;

                    //prevent deactivation of the last active administrator
                    if (!user.IsAdmin() || model.Active || SecondAdminAccountExists(user))
                        user.Active = model.Active;
                    else
                        ErrorNotification(_localizationService.GetResource("Admin.Users.Users.AdminAccountShouldExists.Deactivate"));

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        _userRegistrationService.SetEmail(user, model.Email, false);
                    else
                        user.Email = model.Email;

                    //username
                    if (_userSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            _userRegistrationService.SetUsername(user, model.Username);
                        else
                            user.Username = model.Username;
                    }

                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.GenderAttribute, model.Gender);
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.LastNameAttribute, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.DateOfBirthAttribute, model.DateOfBirth);
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.CompanyAttribute, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.CityAttribute, model.City);
                    if (_userSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.CountyAttribute, model.County);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.CountryIdAttribute, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.PhoneAttribute, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, NopUserDefaults.FaxAttribute, model.Fax);

                    //custom user attributes
                    _genericAttributeService.SaveAttribute(user, NopUserDefaults.CustomUserAttributes, userAttributesXml);

                    //user roles
                    foreach (var userRole in allUserRoles)
                    {
                        //ensure that the current user cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (userRole.SystemName == NopUserDefaults.AdministratorsRoleName &&
                            !_workContext.CurrentUser.IsAdmin())
                            continue;
                    }

                    _userService.UpdateUser(user);

                    //activity log
                    _userActivityService.InsertActivity("EditUser",
                        string.Format(_localizationService.GetResource("ActivityLog.EditUser"), user.Id), user);

                    SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = user.Id });
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc.Message, false);
                }
            }

            //prepare model
            model = _userModelFactory.PrepareUserModel(model, user, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual IActionResult ChangePassword(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            //ensure that the current user cannot change passwords of "Administrators" if he's not an admin himself
            if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.OnlyAdminCanChangePassword"));
                return RedirectToAction("Edit", new { id = user.Id });
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = user.Id });

            var changePassRequest = new ChangePasswordRequest(model.Email,
                false, _userSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
            if (changePassResult.Success)
                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.PasswordChanged"));
            else
                foreach (var error in changePassResult.Errors)
                    ErrorNotification(error);

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public virtual IActionResult MarkVatNumberAsValid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");
            

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public virtual IActionResult MarkVatNumberAsInvalid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");
            

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("remove-affiliate")]
        public virtual IActionResult RemoveAffiliate(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");
            
            _userService.UpdateUser(user);

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(id);
            if (user == null)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (user.IsAdmin() && !SecondAdminAccountExists(user))
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Users.Users.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                //ensure that the current user cannot delete "Administrators" if he's not an admin himself
                if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Users.Users.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                //delete
                _userService.DeleteUser(user);


                //activity log
                _userActivityService.InsertActivity("DeleteUser",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteUser"), user.Id), user);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual IActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowUserImpersonation))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(id);
            if (user == null)
                return RedirectToAction("List");

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_workContext.CurrentUser.IsAdmin() && user.IsAdmin())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.NonAdminNotImpersonateAsAdminError"));
                return RedirectToAction("Edit", user.Id);
            }

            //activity log
            _userActivityService.InsertActivity("Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.Owner"), user.Email, user.Id), user);
            _userActivityService.InsertActivity(user, "Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.User"), _workContext.CurrentUser.Email, _workContext.CurrentUser.Id), _workContext.CurrentUser);

            //ensure login is not required
            user.RequireReLogin = false;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentUser, NopUserDefaults.ImpersonatedUserIdAttribute, user.Id);

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public virtual IActionResult SendWelcomeMessage(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Users.Users.SendWelcomeMessage.Success"));

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("resend-activation-message")]
        public virtual IActionResult ReSendActivationMessage(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            //email validation message
            _genericAttributeService.SaveAttribute(user, NopUserDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
            _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Users.Users.ReSendActivationMessage.Success"));

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public virtual IActionResult SendEmail(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");

            try
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    throw new NopException("User email is empty");
                if (!CommonHelper.IsValidEmail(user.Email))
                    throw new NopException("User email is not valid");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new NopException("Email subject is empty");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new NopException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new NopException("Email account can't be loaded");
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = _userService.GetUserFullName(user),
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                        null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
                };
                _queuedEmailService.InsertQueuedEmail(email);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public virtual IActionResult SendPm(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = user.Id });
        }

        #endregion

        #region Addresses

        [HttpPost]
        public virtual IActionResult AddressesSelect(UserAddressSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            //try to get a user with the specified id
            var user = _userService.GetUserById(searchModel.UserId)
                ?? throw new ArgumentException("No user found with the specified id");

            //prepare model
            var model = _userModelFactory.PrepareUserAddressListModel(searchModel, user);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult AddressDelete(int id, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(userId)
                ?? throw new ArgumentException("No user found with the specified id", nameof(userId));
            
            return new NullJsonResult();
        }

        public virtual IActionResult AddressCreate(int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(userId);
            if (user == null)
                return RedirectToAction("List");

            //prepare model
            var model = _userModelFactory.PrepareUserAddressModel(new UserAddressModel(), user, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressCreate(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(model.Form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                //user.Addresses.Add(address);
                user.UserAddressMappings.Add(new UserAddressMapping { Address = address });
                _userService.UpdateUser(user);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Addresses.Added"));

                return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
            }

            //prepare model
            model = _userModelFactory.PrepareUserAddressModel(model, user, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult AddressEdit(int addressId, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(userId);
            if (user == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                return RedirectToAction("Edit", new { id = user.Id });

            //prepare model
            var model = _userModelFactory.PrepareUserAddressModel(null, user, address);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                return RedirectToAction("Edit", new { id = user.Id });

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(model.Form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Addresses.Updated"));

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
            }

            //prepare model
            model = _userModelFactory.PrepareUserAddressModel(model, user, address, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region User

        public virtual IActionResult LoadUserStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            var searchUserRoleIds = new[] { _userService.GetUserRoleBySystemName(NopUserDefaults.RegisteredRoleName).Id };

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", culture),
                            value = _userService.GetAllUsers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                userRoleIds: searchUserRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", culture),
                            value = _userService.GetAllUsers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                userRoleIds: searchUserRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", culture),
                            value = _userService.GetAllUsers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                userRoleIds: searchUserRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }

        #endregion

        #region Activity log

        [HttpPost]
        public virtual IActionResult ListActivityLog(UserActivityLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            //try to get a user with the specified id
            var user = _userService.GetUserById(searchModel.UserId)
                ?? throw new ArgumentException("No user found with the specified id");

            //prepare model
            var model = _userModelFactory.PrepareUserActivityLogListModel(searchModel, user);

            return Json(model);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(UserSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = _userService.GetAllUsers(loadOnlyWithShoppingCart: false,
                userRoleIds: model.SelectedUserRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
                monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode);

            try
            {
                var xml = _exportManager.ExportUsersToXml(users);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "users.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = new List<User>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                users.AddRange(_userService.GetUsersByIds(ids));
            }

            var xml = _exportManager.ExportUsersToXml(users);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "users.xml");
        }

        #endregion
    }
}