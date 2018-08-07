using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Users;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Areas.Admin.Models.Security;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the security model factory implementation
    /// </summary>
    public partial class SecurityModelFactory : ISecurityModelFactory
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SecurityModelFactory(IUserService userService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._userService = userService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>Permission mapping model</returns>
        public virtual PermissionMappingModel PreparePermissionMappingModel(PermissionMappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var userRoles = _userService.GetAllUserRoles(true);
            model.AvailableUserRoles = userRoles.Select(role => role.ToModel<UserRoleModel>()).ToList();

            foreach (var permissionRecord in _permissionService.GetAllPermissionRecords())
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = _localizationService.GetLocalizedPermissionName(permissionRecord),
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in userRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] = permissionRecord.PermissionRecordUserRoleMappings
                        .Any(mapping => mapping.UserRoleId == role.Id);
                }
            }

            return model;
        }

        #endregion
    }
}