using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value model
    /// </summary>
    public partial class UserAttributeValueModel : BaseNopEntityModel, ILocalizedModel<UserAttributeValueLocalizedModel>
    {
        #region Ctor

        public UserAttributeValueModel()
        {
            Locales = new List<UserAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int UserAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<UserAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class UserAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}