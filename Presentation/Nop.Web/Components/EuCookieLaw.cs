using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Users;
using Nop.Services.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class EuCookieLawViewComponent : NopViewComponent
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly SiteInformationSettings _siteInformationSettings;

        public EuCookieLawViewComponent(IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            SiteInformationSettings siteInformationSettings)
        {
            this._genericAttributeService = genericAttributeService;
            this._workContext = workContext;
            this._siteInformationSettings = siteInformationSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (!_siteInformationSettings.DisplayEuCookieLawWarning)
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if (_workContext.CurrentUser.IsSearchEngineAccount())
                return Content("");
            
            //ignore notification?
            //right now it's used during logout so popup window is not displayed twice
            if (TempData["nop.IgnoreEuCookieLawWarning"] != null && Convert.ToBoolean(TempData["nop.IgnoreEuCookieLawWarning"]))
                return Content("");

            return View();
        }
    }
}