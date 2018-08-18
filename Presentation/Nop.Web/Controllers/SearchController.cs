using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public class SearchController : BasePublicController
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly SiteInformationSettings _siteInformationSettings;

        #endregion

        #region Ctor

        public SearchController(
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            SiteInformationSettings siteInformationSettings)
        {
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._siteInformationSettings = siteInformationSettings;
        }

        #endregion

        #region Methods
        [HttpPost]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public virtual IActionResult Search(string q)
        {
            return View();
        }

        [HttpPost]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public virtual IActionResult SearchTermAutoComplete(string q)
        {
            return Json(new { });
        }

        #endregion
    }
}
