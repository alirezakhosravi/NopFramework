using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Users;
using Nop.Services.Users;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.No)]
    public partial class ProfileController : BasePublicController
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileController(UserSettings userSettings,
            IUserService userService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            this._userSettings = userSettings;
            this._userService = userService;
            this._permissionService = permissionService;
            this._profileModelFactory = profileModelFactory;
        }

        public virtual IActionResult Index(int? id, int? pageNumber)
        {
            if (!_userSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("HomePage");
            }

            var userId = 0;
            if (id.HasValue)
            {
                userId = id.Value;
            }

            var user = _userService.GetUserById(userId);
            if (user == null || user.IsGuest())
            {
                return RedirectToRoute("HomePage");
            }

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                DisplayEditLink(Url.Action("Edit", "User", new { id = user.Id, area = AreaNames.Admin }));

            var model = _profileModelFactory.PrepareProfileIndexModel(user, pageNumber);
            return View(model);
        }
    }
}