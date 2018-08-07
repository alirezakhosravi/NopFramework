using System;
using Nop.Core;
using Nop.Core.Domain.Users;

namespace Nop.Services.Users
{
    /// <summary>
    /// User report service interface
    /// </summary>
    public partial interface IUserReportService
    {
        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered users</returns>
        int GetRegisteredUsersReport(int days);
    }
}