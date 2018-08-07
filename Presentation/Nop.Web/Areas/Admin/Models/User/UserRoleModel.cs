using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Users;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role model
    /// </summary>
    [Validator(typeof(UserRoleValidator))]
    public partial class UserRoleModel : BaseNopEntityModel
    {
        #region Ctor

        public UserRoleModel()
        {
            this.TaxDisplayTypeValues = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.FreeShipping")]
        public bool FreeShipping { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.TaxExempt")]
        public bool TaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.EnablePasswordLifetime")]
        public bool EnablePasswordLifetime { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.OverrideTaxDisplayType")]
        public bool OverrideTaxDisplayType { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.DefaultTaxDisplayType")]
        public int DefaultTaxDisplayTypeId { get; set; }

        public IList<SelectListItem> TaxDisplayTypeValues { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public int PurchasedWithProductId { get; set; }

        [NopResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public string PurchasedWithProductName { get; set; }

        #endregion
    }
}