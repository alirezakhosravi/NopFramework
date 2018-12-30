using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Web.Areas.Admin.Models.Users;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user model factory
    /// </summary>
    public partial interface IUserModelFactory
    {
        /// <summary>
        /// Prepare user search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel);

        /// <summary>
        /// Prepare paged user list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        UserListModel PrepareUserListModel(UserSearchModel searchModel);

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User model</returns>
        UserModel PrepareUserModel(UserModel model, User user, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged user address list model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>User address list model</returns>
        UserAddressListModel PrepareUserAddressListModel(UserAddressSearchModel searchModel, User user);

        /// <summary>
        /// Prepare user address model
        /// </summary>
        /// <param name="model">User address model</param>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User address model</returns>
        UserAddressModel PrepareUserAddressModel(UserAddressModel model,
            User user, Address address, bool excludeProperties = false);
        
        /// <summary>
        /// Prepare paged user activity log list model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>User activity log list model</returns>
        UserActivityLogListModel PrepareUserActivityLogListModel(UserActivityLogSearchModel searchModel, User user);

        /// <summary>
        /// Prepare online user search model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>Online user search model</returns>
        OnlineUserSearchModel PrepareOnlineUserSearchModel(OnlineUserSearchModel searchModel);

        /// <summary>
        /// Prepare paged online user list model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>Online user list model</returns>
        OnlineUserListModel PrepareOnlineUserListModel(OnlineUserSearchModel searchModel);


    }
}