using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Users;
using Nop.Services.Users;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class UserAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly IUserAttributeModelFactory _userAttributeModelFactory;
        private readonly IUserAttributeService _userAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public UserAttributeController(IUserActivityService userActivityService,
            IUserAttributeModelFactory userAttributeModelFactory,
            IUserAttributeService userAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService)
        {
            this._userActivityService = userActivityService;
            this._userAttributeModelFactory = userAttributeModelFactory;
            this._userAttributeService = userAttributeService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateAttributeLocales(UserAttribute userAttribute, UserAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(userAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateValueLocales(UserAttributeValue userAttributeValue, UserAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(userAttributeValue,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region User attributes

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select "user form fields" tab
            SaveSelectedTabName("tab-userformfields");

            //we just redirect a user to the user settings page
            return RedirectToAction("UserUser", "Setting");
        }

        [HttpPost]
        public virtual IActionResult List(UserAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _userAttributeModelFactory.PrepareUserAttributeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _userAttributeModelFactory.PrepareUserAttributeModel(new UserAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(UserAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userAttribute = model.ToEntity<UserAttribute>();
                _userAttributeService.InsertUserAttribute(userAttribute);

                //activity log
                _userActivityService.InsertActivity("AddNewUserAttribute",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewUserAttribute"), userAttribute.Id),
                    userAttribute);

                //locales
                UpdateAttributeLocales(userAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = userAttribute.Id });
            }
            
            //prepare model
            model = _userAttributeModelFactory.PrepareUserAttributeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(id);
            if (userAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _userAttributeModelFactory.PrepareUserAttributeModel(null, userAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(UserAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(model.Id);
            if (userAttribute == null)
                //no user attribute found with the specified id
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                //if we got this far, something failed, redisplay form
                return View(model);

            userAttribute = model.ToEntity(userAttribute);
            _userAttributeService.UpdateUserAttribute(userAttribute);

            //activity log
            _userActivityService.InsertActivity("EditUserAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.EditUserAttribute"), userAttribute.Id),
                userAttribute);

            //locales
            UpdateAttributeLocales(userAttribute, model);

            SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("Edit", new { id = userAttribute.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(id);
            _userAttributeService.DeleteUserAttribute(userAttribute);

            //activity log
            _userActivityService.InsertActivity("DeleteUserAttribute",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteUserAttribute"), userAttribute.Id),
                userAttribute);

            SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region User attribute values

        [HttpPost]
        public virtual IActionResult ValueList(UserAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(searchModel.UserAttributeId)
                ?? throw new ArgumentException("No user attribute found with the specified id");

            //prepare model
            var model = _userAttributeModelFactory.PrepareUserAttributeValueListModel(searchModel, userAttribute);

            return Json(model);
        }

        public virtual IActionResult ValueCreatePopup(int userAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(userAttributeId);
            if (userAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _userAttributeModelFactory
                .PrepareUserAttributeValueModel(new UserAttributeValueModel(), userAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueCreatePopup(UserAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(model.UserAttributeId);
            if (userAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var cav = model.ToEntity<UserAttributeValue>();
                _userAttributeService.InsertUserAttributeValue(cav);

                //activity log
                _userActivityService.InsertActivity("AddNewUserAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewUserAttributeValue"), cav.Id), cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _userAttributeModelFactory.PrepareUserAttributeValueModel(model, userAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute value with the specified id
            var userAttributeValue = _userAttributeService.GetUserAttributeValueById(id);
            if (userAttributeValue == null)
                return RedirectToAction("List");

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(userAttributeValue.UserAttributeId);
            if (userAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = _userAttributeModelFactory.PrepareUserAttributeValueModel(null, userAttribute, userAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueEditPopup(UserAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute value with the specified id
            var userAttributeValue = _userAttributeService.GetUserAttributeValueById(model.Id);
            if (userAttributeValue == null)
                return RedirectToAction("List");

            //try to get a user attribute with the specified id
            var userAttribute = _userAttributeService.GetUserAttributeById(userAttributeValue.UserAttributeId);
            if (userAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                userAttributeValue = model.ToEntity(userAttributeValue);
                _userAttributeService.UpdateUserAttributeValue(userAttributeValue);

                //activity log
                _userActivityService.InsertActivity("EditUserAttributeValue",
                    string.Format(_localizationService.GetResource("ActivityLog.EditUserAttributeValue"), userAttributeValue.Id),
                    userAttributeValue);

                UpdateValueLocales(userAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _userAttributeModelFactory.PrepareUserAttributeValueModel(model, userAttribute, userAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a user attribute value with the specified id
            var userAttributeValue = _userAttributeService.GetUserAttributeValueById(id)
                ?? throw new ArgumentException("No user attribute value found with the specified id", nameof(id));

            _userAttributeService.DeleteUserAttributeValue(userAttributeValue);

            //activity log
            _userActivityService.InsertActivity("DeleteUserAttributeValue",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteUserAttributeValue"), userAttributeValue.Id),
                userAttributeValue);

            return new NullJsonResult();
        }

        #endregion
    }
}