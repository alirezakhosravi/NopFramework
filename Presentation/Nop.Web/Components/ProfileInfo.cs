using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Users;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ProfileInfoViewComponent : NopViewComponent
    {
        private readonly IUserService _userService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileInfoViewComponent(IUserService userService, IProfileModelFactory profileModelFactory)
        {
            this._userService = userService;
            this._profileModelFactory = profileModelFactory;
        }

        public IViewComponentResult Invoke(int userProfileId)
        {
            var user = _userService.GetUserById(userProfileId);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var model = _profileModelFactory.PrepareProfileInfoModel(user);
            return View(model);
        }
    }
}
