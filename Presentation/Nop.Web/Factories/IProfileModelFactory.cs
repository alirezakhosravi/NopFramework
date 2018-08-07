using Nop.Core.Domain.Users;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the profile model factory
    /// </summary>
    public partial interface IProfileModelFactory
    {
        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        ProfileIndexModel PrepareProfileIndexModel(User user, int? page);

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Profile info model</returns>
        ProfileInfoModel PrepareProfileInfoModel(User user);

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>Profile posts model</returns>  
        ProfilePostsModel PrepareProfilePostsModel(User user, int page);
    }
}
