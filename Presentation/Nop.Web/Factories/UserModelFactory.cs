using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Models.Common;
using Nop.Web.Models.User;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the user model factory
    /// </summary>
    public partial class UserModelFactory : IUserModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public UserModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IAddressModelFactory addressModelFactory,
            ICountryService countryService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IExternalAuthenticationService externalAuthenticationService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            IStateProvinceService stateProvinceService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            SecuritySettings securitySettings)
        {
            this._addressSettings = addressSettings;
            this._captchaSettings = captchaSettings;
            this._commonSettings = commonSettings;
            this._userSettings = userSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._addressModelFactory = addressModelFactory;
            this._countryService = countryService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._dateTimeHelper = dateTimeHelper;
            this._downloadService = downloadService;
            this._externalAuthenticationService = externalAuthenticationService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._pictureService = pictureService;
            this._stateProvinceService = stateProvinceService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._securitySettings = securitySettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the custom user attribute models
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>List of the user attribute model</returns>
        public virtual IList<UserAttributeModel> PrepareCustomUserAttributes(User user, string overrideAttributesXml = "")
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var result = new List<UserAttributeModel>();

            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _userAttributeService.GetUserAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new UserAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.CustomUserAttributes);
                
                result.Add(attributeModel);
            }

            return result;
        }

        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>User info model</returns>
        public virtual UserInfoModel PrepareUserInfoModel(UserInfoModel model, User user,
            bool excludeProperties, string overrideCustomUserAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.VatNumberAttribute);
                model.FirstName = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.FirstNameAttribute);
                model.LastName = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.LastNameAttribute);
                model.Gender = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.GenderAttribute);
                var dateOfBirth = _genericAttributeService.GetAttribute<DateTime?>(user, NopUserDefaults.DateOfBirthAttribute);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
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

                model.Signature = _genericAttributeService.GetAttribute<string>(user, NopUserDefaults.SignatureAttribute);

                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }

            if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = user.EmailToRevalidate;

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetLocalized(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountyRequired = _userSettings.CountyRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;


            //external authentication
            model.AllowUsersToRemoveAssociations = _externalAuthenticationSettings.AllowUsersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = _externalAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_workContext.CurrentUser).Count;
            

            //custom user attributes
            var customAttributes = PrepareCustomUserAttributes(user, overrideCustomUserAttributesXml);
            foreach (var attribute in customAttributes)
                model.UserAttributes.Add(attribute);
            
            return model;
        }

        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>User register model</returns>
        public virtual RegisterModel PrepareRegisterModel(RegisterModel model, bool excludeProperties,
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountyRequired = _userSettings.CountyRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.AcceptPrivacyPolicyEnabled = _userSettings.AcceptPrivacyPolicyEnabled;
            model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _userSettings.EnteringEmailTwice;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetLocalized(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom user attributes
            var customAttributes = PrepareCustomUserAttributes(_workContext.CurrentUser, overrideCustomUserAttributesXml); foreach (var attribute in customAttributes)
                model.UserAttributes.Add(attribute);
            
            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        public virtual LoginModel PrepareLoginModel(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = _userSettings.UsernamesEnabled,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
            };
            return model;
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <returns>Password recovery model</returns>
        public virtual PasswordRecoveryModel PreparePasswordRecoveryModel()
        {
            var model = new PasswordRecoveryModel();
            return model;
        }

        /// <summary>
        /// Prepare the password recovery confirm model
        /// </summary>
        /// <returns>Password recovery confirm model</returns>
        public virtual PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel()
        {
            var model = new PasswordRecoveryConfirmModel();
            return model;
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <returns>Register result model</returns>
        public virtual RegisterResultModel PrepareRegisterResultModel(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return model;
        }

        /// <summary>
        /// Prepare the user navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>User navigation model</returns>
        public virtual UserNavigationModel PrepareUserNavigationModel(int selectedTabId = 0)
        {
            var model = new UserNavigationModel();

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserInfo",
                Title = _localizationService.GetResource("Account.UserInfo"),
                Tab = UserNavigationEnum.Info,
                ItemClass = "user-info"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserAddresses",
                Title = _localizationService.GetResource("Account.UserAddresses"),
                Tab = UserNavigationEnum.Addresses,
                ItemClass = "user-addresses"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserOrders",
                Title = _localizationService.GetResource("Account.UserOrders"),
                Tab = UserNavigationEnum.Orders,
                ItemClass = "user-orders"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = UserNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_userSettings.AllowUsersToUploadAvatars)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = UserNavigationEnum.Avatar,
                    ItemClass = "user-avatar"
                });
            }

            if (_captchaSettings.Enabled)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = _localizationService.GetResource("CheckGiftCardBalance"),
                    Tab = UserNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "user-check-gift-card-balance"
                });
            }

            model.SelectedTab = (UserNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>User address list model</returns>
        public virtual UserAddressListModel PrepareUserAddressListModel()
        {
            //var addresses = _workContext.CurrentUser.Addresses
                ////enabled for the current store
                //.Where(a => a.Country == null))
                //.ToList();

            var model = new UserAddressListModel();
            //foreach (var address in addresses)
            //{
            //    var addressModel = new AddressModel();
            //    _addressModelFactory.PrepareAddressModel(addressModel,
            //        address: address,
            //        excludeProperties: false,
            //        addressSettings: _addressSettings,
            //        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));
            //    model.Addresses.Add(addressModel);
            //}
            return model;
        }


        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        public virtual ChangePasswordModel PrepareChangePasswordModel()
        {
            var model = new ChangePasswordModel();
            return model;
        }

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>User avatar model</returns>
        public virtual UserAvatarModel PrepareUserAvatarModel(UserAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = _pictureService.GetPictureUrl(
                _genericAttributeService.GetAttribute<int>(_workContext.CurrentUser, NopUserDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        #endregion
    }
}