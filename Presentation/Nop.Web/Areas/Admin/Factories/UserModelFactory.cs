using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Users;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Users;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user model factory implementation
    /// </summary>
    public partial class UserModelFactory : IUserModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public UserModelFactory(AddressSettings addressSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUserActivityService userActivityService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            MediaSettings mediaSettings)
        {
            this._addressSettings = addressSettings;
            this._userSettings = userSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._aclSupportedModelFactory = aclSupportedModelFactory;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressAttributeModelFactory = addressAttributeModelFactory;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._userActivityService = userActivityService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._externalAuthenticationService = externalAuthenticationService;
            this._genericAttributeService = genericAttributeService;
            this._geoLookupService = geoLookupService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare user associated external authorization models
        /// </summary>
        /// <param name="models">List of user associated external authorization models</param>
        /// <param name="user">User</param>
        protected virtual void PrepareAssociatedExternalAuthModels(IList<UserAssociatedExternalAuthModel> models, User user)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (user == null)
                throw new ArgumentNullException(nameof(user));
        }

        /// <summary>
        /// Prepare user attribute models
        /// </summary>
        /// <param name="models">List of user attribute models</param>
        /// <param name="user">User</param>
        protected virtual void PrepareUserAttributeModels(IList<UserModel.UserAttributeModel> models, User user)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available user attributes
            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserModel.UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _userAttributeService.GetUserAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new UserModel.UserAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (user != null)
                {
                    var selectedUserAttributes = _genericAttributeService
                        .GetAttribute<string>(user, NopUserDefaults.CustomUserAttributes);
                }

                models.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.FirstNameEnabled = true;
            model.FirstNameRequired = true;
            model.LastNameEnabled = true;
            model.LastNameRequired = true;
            model.EmailEnabled = true;
            model.EmailRequired = true;
            model.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.CompanyRequired = _addressSettings.CompanyRequired;
            model.CountryEnabled = _addressSettings.CountryEnabled;
            model.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.CityEnabled = _addressSettings.CityEnabled;
            model.CityRequired = _addressSettings.CityRequired;
            model.CountyEnabled = _addressSettings.CountyEnabled;
            model.CountyRequired = _addressSettings.CountyRequired;
            model.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.PhoneRequired = _addressSettings.PhoneRequired;
            model.FaxEnabled = _addressSettings.FaxEnabled;
            model.FaxRequired = _addressSettings.FaxRequired;

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);

            //prepare custom address attributes
            _addressAttributeModelFactory.PrepareCustomAddressAttributes(model.CustomAddressAttributes, address);
        }

        /// <summary>
        /// Prepare HTML string address
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareModelAddressHtml(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var addressHtmlSb = new StringBuilder("<div>");

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

            if (_addressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

            if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

            if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

            if (_addressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

            if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

            if (_addressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
                addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

            var customAttributesFormatted = _addressAttributeFormatter.FormatAttributes(address?.CustomAttributes);
            if (!string.IsNullOrEmpty(customAttributesFormatted))
            {
                //already encoded
                addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
            }

            addressHtmlSb.Append("</div>");

            model.AddressHtml = addressHtmlSb.ToString();
        }

        /// <summary>
        /// Prepare user address search model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>User address search model</returns>
        protected virtual UserAddressSearchModel PrepareUserAddressSearchModel(UserAddressSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user activity log search model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>User activity log search model</returns>
        protected virtual UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare user search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        public virtual UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = _userSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = _userSettings.AllowUsersToUploadAvatars;
            searchModel.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            searchModel.CompanyEnabled = _userSettings.CompanyEnabled;
            searchModel.PhoneEnabled = _userSettings.PhoneEnabled;
            searchModel.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;

            //search registered users by default
            var registeredRole = _userService.GetUserRoleBySystemName(NopUserDefaults.RegisteredRoleName);
            if (registeredRole != null)
                searchModel.SelectedUserRoleIds.Add(registeredRole.Id);

            //prepare available user roles
            _aclSupportedModelFactory.PrepareModelUserRoles(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        public virtual UserListModel PrepareUserListModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter users
            int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

            //get users
            var users = _userService.GetAllUsers(loadOnlyWithShoppingCart: false,
                userRoleIds: searchModel.SelectedUserRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                phone: searchModel.SearchPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new UserListModel
            {
                Data = users.Select(user =>
                {
                    //fill in model values from the entity
                    var userModel = new UserModel
                    {
                        Id = user.Id,
                        Email = user.IsRegistered() ? user.Email : _localizationService.GetResource("Admin.Users.Guest"),
                        Username = user.Username,
                        FullName = _userService.GetUserFullName(user),
                        Company = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CompanyAttribute),
                        Phone = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.PhoneAttribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.ZipPostalCodeAttribute),
                        Active = user.Active
                    };

                    //convert dates to the user time
                    userModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    userModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    userModel.UserRoleNames = string.Join(", ", user.UserRoles.Select(role => role.Name));
                    if (_userSettings.AllowUsersToUploadAvatars)
                    {
                        var avatarPictureId = _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.AvatarPictureIdAttribute);
                        userModel.AvatarUrl = _pictureService.GetPictureUrl(avatarPictureId, _mediaSettings.AvatarPictureSize,
                            _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    return userModel;
                }),
                Total = users.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User model</returns>
        public virtual UserModel PrepareUserModel(UserModel model, User user, bool excludeProperties = false)
        {
            if (user != null)
            {
                //fill in model values from the entity
                model = model ?? new UserModel();

                model.Id = user.Id;
                model.AllowSendingOfWelcomeMessage = user.IsRegistered() &&
                    _userSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = user.IsRegistered() && !user.Active &&
                    _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation;

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = user.Email;
                    model.Username = user.Username;
                    model.AdminComment = user.AdminComment;
                    model.Active = user.Active;
                    model.FirstName = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FirstNameAttribute);
                    model.LastName = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastNameAttribute);
                    model.Gender = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.GenderAttribute);
                    model.DateOfBirth = _genericAttributeService.GetAttribute<DateTime?>(user, NopUserDefaults.DateOfBirthAttribute);
                    model.Company = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CompanyAttribute);
                    model.StreetAddress = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.StreetAddressAttribute);
                    model.StreetAddress2 = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.StreetAddress2Attribute);
                    model.ZipPostalCode = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.ZipPostalCodeAttribute);
                    model.City = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CityAttribute);
                    model.County = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CountyAttribute);
                    model.CountryId = _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.CountryIdAttribute);
                    model.StateProvinceId = _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.StateProvinceIdAttribute);
                    model.Phone = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.PhoneAttribute);
                    model.Fax = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FaxAttribute);
                    model.TimeZoneId = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.TimeZoneIdAttribute);
                    model.VatNumber = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.VatNumberAttribute);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = user.LastIpAddress;
                    model.LastVisitedPage = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastVisitedPageAttribute);
                }

                //prepare external authentication records
                PrepareAssociatedExternalAuthModels(model.AssociatedExternalAuthRecords, user);

                //prepare nested search models
                PrepareUserAddressSearchModel(model.UserAddressSearchModel, user);
                PrepareUserActivityLogSearchModel(model.UserActivityLogSearchModel, user);

                model.SelectedUserRoleIds = user.UserUserRoleMappings.Select(mapping => mapping.UserRoleId).ToList();
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Registered Role as a default role while creating a new user through admin
                    var registeredRole = _userService.GetUserRoleBySystemName(NopUserDefaults.RegisteredRoleName);
                    if (registeredRole != null)
                        model.SelectedUserRoleIds.Add(registeredRole.Id);
                }
            }

            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            //set default values for the new model
            if (user == null)
            {
                model.Active = true;
                model.DisplayVatNumber = false;
            }

            //prepare model user attributes
            PrepareUserAttributeModels(model.UserAttributes, user);

            //prepare model user roles
            _aclSupportedModelFactory.PrepareModelUserRoles(model);

            //prepare available time zones
            _baseAdminModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            //prepare available countries and states
            if (_userSettings.CountryEnabled)
            {
                _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);
                if (_userSettings.StateProvinceEnabled)
                    _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
            }

            return model;
        }

        /// <summary>
        /// Prepare paged user address list model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>User address list model</returns>
        public virtual UserAddressListModel PrepareUserAddressListModel(UserAddressSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user addresses
            var addresses = user.Addresses
                .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList();

            //prepare list model
            var model = new UserAddressListModel
            {
                Data = addresses.PaginationByRequestModel(searchModel).Select(address =>
                {
                    //fill in model values from the entity        
                    var addressModel = address.ToModel<AddressModel>();

                    //fill in additional values (not existing in the entity)
                    PrepareModelAddressHtml(addressModel, address);

                    return addressModel;
                }),
                Total = addresses.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare user address model
        /// </summary>
        /// <param name="model">User address model</param>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User address model</returns>
        public virtual UserAddressModel PrepareUserAddressModel(UserAddressModel model,
            User user, Address address, bool excludeProperties = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (address != null)
            {
                //fill in model values from the entity
                model = model ?? new UserAddressModel();

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.Address = address.ToModel(model.Address);
            }

            model.UserId = user.Id;

            //prepare address model
            PrepareAddressModel(model.Address, address);

            return model;
        }

        /// <summary>
        /// Prepare paged user activity log list model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>User activity log list model</returns>
        public virtual UserActivityLogListModel PrepareUserActivityLogListModel(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user activity log
            var activityLog = _userActivityService.GetAllActivities(userId: user.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new UserActivityLogListModel
            {
                Data = activityLog.Select(logItem =>
                {
                    //fill in model values from the entity
                    var userActivityLogModel = new UserActivityLogModel
                    {
                        Id = logItem.Id,
                        ActivityLogTypeName = logItem.ActivityLogType.Name,
                        Comment = logItem.Comment,
                        IpAddress = logItem.IpAddress
                    };

                    //convert dates to the user time
                    userActivityLogModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return userActivityLogModel;
                }),
                Total = activityLog.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare online user search model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>Online user search model</returns>
        public virtual OnlineUserSearchModel PrepareOnlineUserSearchModel(OnlineUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged online user list model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>Online user list model</returns>
        public virtual OnlineUserListModel PrepareOnlineUserListModel(OnlineUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter users
            var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_userSettings.OnlineUserMinutes);

            //get online users
            var users = _userService.GetOnlineUsers(userRoleIds: null,
                 lastActivityFromUtc: lastActivityFrom,
                 pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new OnlineUserListModel
            {
                Data = users.Select(user =>
                {
                    //fill in model values from the entity
                    var userModel = new OnlineUserModel
                    {
                        Id = user.Id
                    };

                    //convert dates to the user time
                    userModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    userModel.UserInfo = user.IsRegistered()
                        ? user.Email : _localizationService.GetResource("Admin.Users.Guest");
                    userModel.LastIpAddress = _userSettings.StoreIpAddresses
                        ? user.LastIpAddress : _localizationService.GetResource("Admin.Users.OnlineUsers.Fields.IPAddress.Disabled");
                    userModel.Location = _geoLookupService.LookupCountryName(user.LastIpAddress);
                    userModel.LastVisitedPage = _userSettings.StoreLastVisitedPage
                        ? _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastVisitedPageAttribute)
                        : _localizationService.GetResource("Admin.Users.OnlineUsers.Fields.LastVisitedPage.Disabled");

                    return userModel;
                }),
                Total = users.TotalCount
            };

            return model;
        }

        #endregion
    }
}