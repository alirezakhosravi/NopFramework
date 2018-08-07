using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a registered users report model
    /// </summary>
    public partial class RegisteredUsersReportModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Reports.Users.RegisteredUsers.Fields.Period")]
        public string Period { get; set; }

        [NopResourceDisplayName("Admin.Reports.Users.RegisteredUsers.Fields.Users")]
        public int Users { get; set; }

        #endregion
    }
}