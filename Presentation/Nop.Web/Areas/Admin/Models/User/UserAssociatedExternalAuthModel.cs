using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user associated external authentication model
    /// </summary>
    public partial class UserAssociatedExternalAuthModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.ExternalIdentifier")]
        public string ExternalIdentifier { get; set; }
        
        [NopResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.AuthMethodName")]
        public string AuthMethodName { get; set; }

        #endregion
    }
}