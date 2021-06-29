using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute model
    /// </summary>
    public partial class UserAttributeModel : BaseNopEntityModel, ILocalizedModel<UserAttributeLocalizedModel>
    {
        #region Ctor

        public UserAttributeModel()
        {
            Locales = new List<UserAttributeLocalizedModel>();
            UserAttributeValueSearchModel = new UserAttributeValueSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<UserAttributeLocalizedModel> Locales { get; set; }

        public UserAttributeValueSearchModel UserAttributeValueSearchModel { get; set; }

        #endregion
    }

    public partial class UserAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        public string Name { get; set; }
    }
}