using System.Collections.Generic;
using Nop.Core.Domain.Users;
using Nop.Web.Models.User;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the user model factory
    /// </summary>
    public partial interface IUserModelFactory
    {
        /// <summary>
        /// Prepare the custom user attribute models
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>List of the user attribute model</returns>
        IList<UserAttributeModel> PrepareCustomUserAttributes(User user, string overrideAttributesXml = "");

        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>User info model</returns>
        UserInfoModel PrepareUserInfoModel(UserInfoModel model, User user, 
            bool excludeProperties, string overrideCustomUserAttributesXml = "");

        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>User register model</returns>
        RegisterModel PrepareRegisterModel(RegisterModel model, bool excludeProperties, 
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false);

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        LoginModel PrepareLoginModel(bool? checkoutAsGuest);

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <returns>Password recovery model</returns>
        PasswordRecoveryModel PreparePasswordRecoveryModel();

        /// <summary>
        /// Prepare the password recovery confirm model
        /// </summary>
        /// <returns>Password recovery confirm model</returns>
        PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel();

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <returns>Register result model</returns>
        RegisterResultModel PrepareRegisterResultModel(int resultId);

        /// <summary>
        /// Prepare the user navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>User navigation model</returns>
        UserNavigationModel PrepareUserNavigationModel(int selectedTabId = 0);

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>User address list model</returns>  
        UserAddressListModel PrepareUserAddressListModel();

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        ChangePasswordModel PrepareChangePasswordModel();

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>User avatar model</returns>
        UserAvatarModel PrepareUserAvatarModel(UserAvatarModel model);

    }
}
