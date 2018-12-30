using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class UserNavigationViewComponent : NopViewComponent
    {
        private readonly IUserModelFactory _userModelFactory;

        public UserNavigationViewComponent(IUserModelFactory userModelFactory)
        {
            this._userModelFactory = userModelFactory;
        }

        public IViewComponentResult Invoke(int selectedTabId = 0)
        {
            var model = _userModelFactory.PrepareUserNavigationModel(selectedTabId);
            return View(model);
        }
    }
}
