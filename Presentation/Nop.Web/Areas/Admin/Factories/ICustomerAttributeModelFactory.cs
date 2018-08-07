using Nop.Core.Domain.Users;
using Nop.Web.Areas.Admin.Models.Users;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user attribute model factory
    /// </summary>
    public partial interface IUserAttributeModelFactory
    {
        /// <summary>
        /// Prepare user attribute search model
        /// </summary>
        /// <param name="searchModel">User attribute search model</param>
        /// <returns>User attribute search model</returns>
        UserAttributeSearchModel PrepareUserAttributeSearchModel(UserAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged user attribute list model
        /// </summary>
        /// <param name="searchModel">User attribute search model</param>
        /// <returns>User attribute list model</returns>
        UserAttributeListModel PrepareUserAttributeListModel(UserAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare user attribute model
        /// </summary>
        /// <param name="model">User attribute model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User attribute model</returns>
        UserAttributeModel PrepareUserAttributeModel(UserAttributeModel model,
            UserAttribute userAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged user attribute value list model
        /// </summary>
        /// <param name="searchModel">User attribute value search model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <returns>User attribute value list model</returns>
        UserAttributeValueListModel PrepareUserAttributeValueListModel(UserAttributeValueSearchModel searchModel,
            UserAttribute userAttribute);

        /// <summary>
        /// Prepare user attribute value model
        /// </summary>
        /// <param name="model">User attribute value model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <param name="userAttributeValue">User attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User attribute value model</returns>
        UserAttributeValueModel PrepareUserAttributeValueModel(UserAttributeValueModel model,
            UserAttribute userAttribute, UserAttributeValue userAttributeValue, bool excludeProperties = false);
    }
}