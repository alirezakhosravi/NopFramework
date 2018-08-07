using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the report model factory
    /// </summary>
    public partial interface IReportModelFactory
    {
        #region User reports

        /// <summary>
        /// Prepare user reports search model
        /// </summary>
        /// <param name="searchModel">User reports search model</param>
        /// <returns>User reports search model</returns>
        UserReportsSearchModel PrepareUserReportsSearchModel(UserReportsSearchModel searchModel);

        /// <summary>
        /// Prepare paged registered users report list model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>Registered users report list model</returns>
        RegisteredUsersReportListModel PrepareRegisteredUsersReportListModel(RegisteredUsersReportSearchModel searchModel);

        #endregion
    }
}
