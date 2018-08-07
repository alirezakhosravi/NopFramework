using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReportController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IReportModelFactory _reportModelFactory;

        #endregion

        #region Ctor

        public ReportController(
            IPermissionService permissionService,
            IReportModelFactory reportModelFactory)
        {
            this._permissionService = permissionService;
            this._reportModelFactory = reportModelFactory;
        }

        #endregion

        #region Methods

        #region User reports

        public virtual IActionResult Users()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareUserReportsSearchModel(new UserReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReportRegisteredUsersList(RegisteredUsersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareRegisteredUsersReportListModel(searchModel);

            return Json(model);
        }        

        #endregion

        #endregion
    }
}
