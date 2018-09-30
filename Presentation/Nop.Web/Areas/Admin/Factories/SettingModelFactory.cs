using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory implementation
    /// </summary>
    public partial class SettingModelFactory : ISettingModelFactory
    {
        #region Fields

        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserAttributeModelFactory _userAttributeModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IFulltextService _fulltextService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IThemeProvider _themeProvider;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SettingModelFactory(IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUserAttributeModelFactory userAttributeModelFactory,
            IDateTimeHelper dateTimeHelper,
            IFulltextService fulltextService,
            ILocalizedModelFactory localizedModelFactory,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            IPictureService pictureService,
            ISettingService settingService,
            IThemeProvider themeProvider,
            IWorkContext workContext)
        {
            this._addressAttributeModelFactory = addressAttributeModelFactory;
            this._addressService = addressService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._userAttributeModelFactory = userAttributeModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._fulltextService = fulltextService;
            this._localizedModelFactory = localizedModelFactory;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._maintenanceService = maintenanceService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._themeProvider = themeProvider;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

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
            model.CountryEnabled = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CityEnabled = true;
            model.StreetAddressEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option search model</returns>
        protected virtual SortOptionSearchModel PrepareSortOptionSearchModel(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare address settings model
        /// </summary>
        /// <returns>Address settings model</returns>
        protected virtual AddressSettingsModel PrepareAddressSettingsModel()
        {
            //load settings for a chosen store scope
            var addressSettings = _settingService.LoadSetting<AddressSettings>();

            //fill in model values from the entity
            var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare user settings model
        /// </summary>
        /// <returns>User settings model</returns>
        protected virtual UserSettingsModel PrepareUserSettingsModel()
        {
            //load settings for a chosen store scope
            var userSettings = _settingService.LoadSetting<UserSettings>();

            //fill in model values from the entity
            var model = userSettings.ToSettingsModel<UserSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>Date time settings model</returns>
        protected virtual DateTimeSettingsModel PrepareDateTimeSettingsModel()
        {
            //load settings for a chosen store scope
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();

            //fill in model values from the entity
            var model = new DateTimeSettingsModel
            {
                AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone
            };

            //fill in additional values (not existing in the entity)
            model.DefaultTimeZoneId = _dateTimeHelper.DefaultTimeZone.Id;

            //prepare available time zones
            _baseAdminModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            return model;
        }

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>External authentication settings model</returns>
        protected virtual ExternalAuthenticationSettingsModel PrepareExternalAuthenticationSettingsModel()
        {
            //load settings for a chosen store scope
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            //fill in model values from the entity
            var model = new ExternalAuthenticationSettingsModel
            {
                AllowUsersToRemoveAssociations = externalAuthenticationSettings.AllowUsersToRemoveAssociations
            };

            return model;
        }

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>SEO settings model</returns>
        protected virtual SeoSettingsModel PrepareSeoSettingsModel()
        {
            //load settings for a chosen store scope
            var seoSettings = _settingService.LoadSetting<SeoSettings>();

            //fill in model values from the entity
            var model = new SeoSettingsModel
            {
                PageTitleSeparator = seoSettings.PageTitleSeparator,
                PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
                PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList(),
                DefaultTitle = seoSettings.DefaultTitle,
                DefaultMetaKeywords = seoSettings.DefaultMetaKeywords,
                DefaultMetaDescription = seoSettings.DefaultMetaDescription,
                GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
                ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
                CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
                WwwRequirement = (int)seoSettings.WwwRequirement,
                WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList(),
                EnableJsBundling = seoSettings.EnableJsBundling,
                EnableCssBundling = seoSettings.EnableCssBundling,
                TwitterMetaTags = seoSettings.TwitterMetaTags,
                OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
                CustomHeadTags = seoSettings.CustomHeadTags
            };

            //fill in overridden values
            model.PageTitleSeparator_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator);
            model.PageTitleSeoAdjustment_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment);
            model.DefaultTitle_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultTitle);
            model.DefaultMetaKeywords_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords);
            model.DefaultMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription);
            model.GenerateProductMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.GenerateProductMetaDescription);
            model.ConvertNonWesternChars_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars);
            model.CanonicalUrlsEnabled_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled);
            model.WwwRequirement_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.WwwRequirement);
            model.EnableJsBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableJsBundling);
            model.EnableCssBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableCssBundling);
            model.TwitterMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags);
            model.OpenGraphMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags);
            model.CustomHeadTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CustomHeadTags);

            return model;
        }

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>Security settings model</returns>
        protected virtual SecuritySettingsModel PrepareSecuritySettingsModel()
        {
            //load settings for a chosen store scope
            var securitySettings = _settingService.LoadSetting<SecuritySettings>();

            //fill in model values from the entity
            var model = new SecuritySettingsModel
            {
                EncryptionKey = securitySettings.EncryptionKey,
                ForceSslForAllPages = securitySettings.ForceSslForAllPages,
                EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea,
                EnableXsrfProtectionForPublicSite = securitySettings.EnableXsrfProtectionForSite,
                HoneypotEnabled = securitySettings.HoneypotEnabled
            };

            //fill in additional values (not existing in the entity)
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

            return model;
        }

        /// <summary>
        /// Prepare captcha settings model
        /// </summary>
        /// <returns>Captcha settings model</returns>
        protected virtual CaptchaSettingsModel PrepareCaptchaSettingsModel()
        {
            //load settings for a chosen store scope
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>();

            //fill in model values from the entity
            var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>PDF settings model</returns>
        protected virtual PdfSettingsModel PreparePdfSettingsModel()
        {
            //load settings for a chosen store scope
            var pdfSettings = _settingService.LoadSetting<PdfSettings>();

            //fill in model values from the entity
            var model = new PdfSettingsModel
            {
                LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
                LogoPictureId = pdfSettings.LogoPictureId,
            };

            //fill in overridden values
            model.LetterPageSizeEnabled_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LetterPageSizeEnabled);
            model.LogoPictureId_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LogoPictureId);

            return model;
        }

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>Localization settings model</returns>
        protected virtual LocalizationSettingsModel PrepareLocalizationSettingsModel()
        {
            //load settings for a chosen store scope
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>();

            //fill in model values from the entity
            var model = new LocalizationSettingsModel
            {
                UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
                SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
                AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
                LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
                LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
                LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
            };

            return model;
        }

        /// <summary>
        /// Prepare full-text settings model
        /// </summary>
        /// <returns>Full-text settings model</returns>
        protected virtual FullTextSettingsModel PrepareFullTextSettingsModel()
        {
            //load settings for a chosen store scope
            var commonSettings = _settingService.LoadSetting<CommonSettings>();

            //fill in model values from the entity
            var model = new FullTextSettingsModel
            {
                Enabled = commonSettings.UseFullTextSearch,
                SearchMode = (int)commonSettings.FullTextMode
            };

            //fill in additional values (not existing in the entity)
            model.Supported = _fulltextService.IsFullTextSupported();
            model.SearchModeValues = commonSettings.FullTextMode.ToSelectList();

            return model;
        }

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>Admin area settings model</returns>
        protected virtual AdminAreaSettingsModel PrepareAdminAreaSettingsModel()
        {
            //load settings for a chosen store scope
            var adminAreaSettings = _settingService.LoadSetting<AdminAreaSettings>();

            //fill in model values from the entity
            var model = new AdminAreaSettingsModel
            {
                UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
            };

            return model;
        }

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>Display default menu item settings model</returns>
        protected virtual DisplayDefaultMenuItemSettingsModel PrepareDisplayDefaultMenuItemSettingsModel()
        {
            //load settings for a chosen store scope
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>();

            //fill in model values from the entity
            var model = new DisplayDefaultMenuItemSettingsModel
            {
                DisplayHomePageMenuItem = displayDefaultMenuItemSettings.DisplayHomePageMenuItem,
                DisplayUserInfoMenuItem = displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
                DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem
            };

            return model;
        }

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>Display default footer item settings model</returns>
        protected virtual DisplayDefaultFooterItemSettingsModel PrepareDisplayDefaultFooterItemSettingsModel()
        {
            //load settings for a chosen store scope
            var displayDefaultFooterItemSettings = _settingService.LoadSetting<DisplayDefaultFooterItemSettings>();

            //fill in model values from the entity
            var model = new DisplayDefaultFooterItemSettingsModel
            {
                DisplaySitemapFooterItem = displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayUserInfoFooterItem = displayDefaultFooterItemSettings.DisplayUserInfoFooterItem,
                DisplayUserAddressesFooterItem = displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem,
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        public virtual SortOptionListModel PrepareSortOptionListModel(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            
            //prepare list model
            var model = new SortOptionListModel
            {
                
            };

            return model;
        }

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        public virtual MediaSettingsModel PrepareMediaSettingsModel()
        {
            var mediaSettings = _settingService.LoadSetting<MediaSettings>();

            //fill in model values from the entity
            var model = mediaSettings.ToSettingsModel<MediaSettingsModel>();

            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            return model;
        }

        /// <summary>
        /// Prepare user user settings model
        /// </summary>
        /// <returns>User user settings model</returns>
        public virtual UserUserSettingsModel PrepareUserUserSettingsModel()
        {
            var model = new UserUserSettingsModel();

            //prepare user settings model
            model.UserSettings = PrepareUserSettingsModel();

            //prepare address settings model
            model.AddressSettings = PrepareAddressSettingsModel();

            //prepare date time settings model
            model.DateTimeSettings = PrepareDateTimeSettingsModel();

            //prepare external authentication settings model
            model.ExternalAuthenticationSettings = PrepareExternalAuthenticationSettingsModel();

            //prepare nested search models
            _userAttributeModelFactory.PrepareUserAttributeSearchModel(model.UserAttributeSearchModel);
            _addressAttributeModelFactory.PrepareAddressAttributeSearchModel(model.AddressAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        public virtual GeneralCommonSettingsModel PrepareGeneralCommonSettingsModel()
        {
            var model = new GeneralCommonSettingsModel();

            //prepare SEO settings model
            model.SeoSettings = PrepareSeoSettingsModel();

            //prepare security settings model
            model.SecuritySettings = PrepareSecuritySettingsModel();

            //prepare captcha settings model
            model.CaptchaSettings = PrepareCaptchaSettingsModel();

            //prepare PDF settings model
            model.PdfSettings = PreparePdfSettingsModel();

            //prepare PDF settings model
            model.LocalizationSettings = PrepareLocalizationSettingsModel();

            //prepare full-text settings model
            model.FullTextSettings = PrepareFullTextSettingsModel();

            //prepare admin area settings model
            model.AdminAreaSettings = PrepareAdminAreaSettingsModel();

            //prepare display default menu item settings model
            model.DisplayDefaultMenuItemSettings = PrepareDisplayDefaultMenuItemSettingsModel();

            //prepare display default footer item settings model
            model.DisplayDefaultFooterItemSettings = PrepareDisplayDefaultFooterItemSettingsModel();

            return model;
        }


        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        public virtual SettingSearchModel PrepareSettingSearchModel(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        public virtual SettingListModel PrepareSettingListModel(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get settings
            var settings = _settingService.GetAllSettings().AsQueryable();

            //filter settings
            //TODO: move filter to setting service
            if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
                settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
                settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

            //prepare list model
            var model = new SettingListModel
            {
                Data = settings.PaginationByRequestModel(searchModel).Select(setting =>
                {
                    //fill in model values from the entity
                    var settingModel = new SettingModel
                    {
                        Id = setting.Id,
                        Name = setting.Name,
                        Value = setting.Value,
                    };

                    return settingModel;
                }),

                Total = settings.Count()
            };

            return model;
        }

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        public virtual SettingModeModel PrepareSettingModeModel(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentUser, modeName)
            };

            return model;
        }



        #endregion
    }
}