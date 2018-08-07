using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log model
    /// </summary>
    public partial class ActivityLogModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public int UserId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public string UserEmail { get; set; }

        [NopResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }

        [NopResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
        public string IpAddress { get; set; }

        #endregion
    }
}