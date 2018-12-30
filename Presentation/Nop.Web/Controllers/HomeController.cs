using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Services.Notifications;
using Nop.Core.Data;
using Nop.Core.Domain.Users;
using Nop.Data.Extensions;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        private readonly IWorkflowNotificationService _workflowNotificationService;
        public HomeController(IWorkflowNotificationService workflowNotificationService)
        {
            _workflowNotificationService = workflowNotificationService;

        }
        [HttpsRequirement(SslRequirement.NoMatter)]
        public virtual IActionResult Index()
        {
            _workflowNotificationService.SendTestNotification(1, string.Empty, new System.Collections.Generic.List<Services.Messages.Token>(), 1);
            return View();
        }
    }
}