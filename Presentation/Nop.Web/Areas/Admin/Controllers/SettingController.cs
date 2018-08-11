using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Users;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly IFulltextService _fulltextService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly NopConfig _config;

        #endregion

        #region Ctor

        public SettingController(IAddressService addressService,
            IUserActivityService userActivityService,
            IUserService userService,
            IEncryptionService encryptionService,
            IFulltextService fulltextService,
            IGenericAttributeService genericAttributeService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingModelFactory settingModelFactory,
            ISettingService settingService,
            IWorkContext workContext,
            NopConfig config)
        {
            this._addressService = addressService;
            this._userActivityService = userActivityService;
            this._userService = userService;
            this._encryptionService = encryptionService;
            this._fulltextService = fulltextService;
            this._genericAttributeService = genericAttributeService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._maintenanceService = maintenanceService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._settingModelFactory = settingModelFactory;
            this._settingService = settingService;
            this._workContext = workContext;
            this._config = config;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult SortOptionsList(SortOptionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _settingModelFactory.PrepareSortOptionListModel(searchModel);

            return Json(model);
        }
        [HttpPost]
        public virtual IActionResult SortOptionUpdate(SortOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return new NullJsonResult();
        }

        public virtual IActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _settingModelFactory.PrepareMediaSettingsModel();

            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var mediaSettings = _settingService.LoadSetting<MediaSettings>();
            mediaSettings = model.ToSettings(mediaSettings);

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            _settingService.SaveSetting(mediaSettings, x => x.AvatarPictureSize, true);
            _settingService.SaveSetting(mediaSettings, x => x.MaximumImageSize, false);
            _settingService.SaveSetting(mediaSettings, x => x.MultipleThumbDirectories, false);
            _settingService.SaveSetting(mediaSettings, x => x.DefaultImageQuality, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public virtual IActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("Media");
        }

        public virtual IActionResult UserUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _settingModelFactory.PrepareUserUserSettingsModel();

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult UserUser(UserUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            var userSettings = _settingService.LoadSetting<UserSettings>();

            var lastUsernameValidationRule = userSettings.UsernameValidationRule;
            var lastUsernameValidationEnabledValue = userSettings.UsernameValidationEnabled;
            var lastUsernameValidationUseRegexValue = userSettings.UsernameValidationUseRegex;

            var addressSettings = _settingService.LoadSetting<AddressSettings>();
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            userSettings = model.UserSettings.ToSettings(userSettings);

            if (userSettings.UsernameValidationEnabled && userSettings.UsernameValidationUseRegex)
            {
                try
                {
                    //validate regex rule
                    var unused = Regex.IsMatch("test_user_name", userSettings.UsernameValidationRule);
                }
                catch (ArgumentException)
                {
                    //restoring previous settings
                    userSettings.UsernameValidationRule = lastUsernameValidationRule;
                    userSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
                    userSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Settings.UserSettings.RegexValidationRule.Error"));
                }
            }

            _settingService.SaveSetting(userSettings);

            addressSettings = model.AddressSettings.ToSettings(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AllowUsersToRemoveAssociations = model.ExternalAuthenticationSettings.AllowUsersToRemoveAssociations;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("UserUser");
        }



        public virtual IActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _settingModelFactory.PrepareGeneralCommonSettingsModel();

            //notify admin that CSS bundling is not allowed in virtual directories
            if (model.SeoSettings.EnableCssBundling && this.HttpContext.Request.PathBase.HasValue)
                WarningNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling.Warning"), false);

            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public virtual IActionResult ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = string.Empty;

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (string.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new NopException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));
                
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public virtual IActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return RedirectToAction("GeneralCommon");
        }

        public virtual IActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _settingModelFactory.PrepareSettingSearchModel(new SettingSearchModel());

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult AllSettings(SettingSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _settingModelFactory.PrepareSettingListModel(searchModel);

            return Json(model);
        }
        [HttpPost]
        public virtual IActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();

            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            //try to get a setting with the specified id
            var setting = _settingService.GetSettingById(model.Id)
                ?? throw new ArgumentException("No setting found with the specified id");
            
            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"), setting);

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual IActionResult SettingAdd(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();

            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            
            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _userActivityService.InsertActivity("AddNewSetting",
                string.Format(_localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name),
                _settingService.GetSetting(model.Name));

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual IActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a setting with the specified id
            var setting = _settingService.GetSettingById(id)
                ?? throw new ArgumentException("No setting found with the specified id", nameof(id));

            _settingService.DeleteSetting(setting);

            //activity log
            _userActivityService.InsertActivity("DeleteSetting",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name), setting);

            return new NullJsonResult();
        }

        //action displaying notification (warning) to a store owner about a lot of traffic 
        //between the Redis server and the application when LoadAllLocaleRecordsOnStartup setting is set
        public IActionResult RedisCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
        {
            //LoadAllLocaleRecordsOnStartup is set and Redis cache is used, so display warning
            if (_config.RedisCachingEnabled && loadAllLocaleRecordsOnStartup)
                return Json(new
                {
                    Result = _localizationService.GetResource(
                        "Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning")
                });

            return Json(new { Result = string.Empty });
        }

        #endregion
    }
}