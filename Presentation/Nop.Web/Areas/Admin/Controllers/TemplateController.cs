using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Templates;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITemplateModelFactory _templateModelFactory;

        #endregion

        #region Ctor

        public TemplateController(IPermissionService permissionService,
            ITemplateModelFactory templateModelFactory)
        {
            this._permissionService = permissionService;
            this._templateModelFactory = templateModelFactory;
        }

        #endregion

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = _templateModelFactory.PrepareTemplatesModel(new TemplatesModel());

            return View(model);
        }
        #endregion
    }
}