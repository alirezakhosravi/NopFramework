using System;
using System.Linq;

namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// User extensions
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Gets a value indicating whether User is in a certain User role
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="UserRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User User,
            string UserRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (string.IsNullOrEmpty(UserRoleSystemName))
                throw new ArgumentNullException(nameof(UserRoleSystemName));

            var result = User.UserRoles
                .FirstOrDefault(cr => (!onlyActiveUserRoles || cr.Active) && cr.SystemName == UserRoleSystemName) != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether User a search engine
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (!User.IsSystemAccount || string.IsNullOrEmpty(User.SystemName))
                return false;

            var result = User.SystemName.Equals(NopUserDefaults.SearchEngineUserName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the User is a built-in record for background tasks
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (!User.IsSystemAccount || string.IsNullOrEmpty(User.SystemName))
                return false;

            var result = User.SystemName.Equals(NopUserDefaults.BackgroundTaskUserName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether User is administrator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, NopUserDefaults.AdministratorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is registered
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, NopUserDefaults.RegisteredRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is guest
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsGuest(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, NopUserDefaults.GuestsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Get User role identifiers
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>User role identifiers</returns>
        public static int[] GetUserRoleIds(this User User, bool showHidden = false)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var UserRolesIds = User.UserRoles
               .Where(cr => showHidden || cr.Active)
               .Select(cr => cr.Id)
               .ToArray();

            return UserRolesIds;
        }
    }
}