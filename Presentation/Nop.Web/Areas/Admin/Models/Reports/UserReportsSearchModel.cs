using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a user reports search model
    /// </summary>
    public partial class UserReportsSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserReportsSearchModel()
        {
            RegisteredUsers = new RegisteredUsersReportSearchModel();
        }

        #endregion

        #region Properties

        public RegisteredUsersReportSearchModel RegisteredUsers { get; set; }

        #endregion
    }
}