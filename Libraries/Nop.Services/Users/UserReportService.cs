using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Users;
using Nop.Services.Helpers;

namespace Nop.Services.Users
{
    /// <summary>
    /// User report service
    /// </summary>
    public partial class UserReportService : IUserReportService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<User> _userRepository;

        #endregion

        #region Ctor

        public UserReportService(IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IRepository<User> userRepository)
        {
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._userRepository = userRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered users</returns>
        public virtual int GetRegisteredUsersReport(int days)
        {
            var date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredUserRole = _userService.GetUserRoleBySystemName(NopUserDefaults.RegisteredRoleName);
            if (registeredUserRole == null)
                return 0;

            var query = from c in _userRepository.Table
                        where !c.Deleted &&
                        c.CreatedOnUtc >= date
                        //&& c.CreatedOnUtc <= DateTime.UtcNow
                        select c;

            var count = query.Count();
            return count;
        }

        #endregion
    }
}