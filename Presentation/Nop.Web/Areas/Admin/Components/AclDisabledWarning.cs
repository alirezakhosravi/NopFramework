using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class AclDisabledWarningViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;

        public AclDisabledWarningViewComponent(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        public IViewComponentResult Invoke()
        {
            //action displaying notification (warning) to a site owner that "ACL rules" feature is ignored

            return View();
        }
    }
}
