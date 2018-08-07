using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial class UserSearchModel : BaseSearchModel, IAclSupportedModel
    {
        #region Ctor

        public UserSearchModel()
        {
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
        public string SearchUsername { get; set; }

        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
        public string SearchFirstName { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        public string SearchDayOfBirth { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        public string SearchMonthOfBirth { get; set; }

        public bool DateOfBirthEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchCompany")]
        public string SearchCompany { get; set; }

        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchPhone")]
        public string SearchPhone { get; set; }

        public bool PhoneEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
        public string SearchZipPostalCode { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.List.SearchIpAddress")]
        public string SearchIpAddress { get; set; }

        public bool AvatarEnabled { get; internal set; }

        #endregion
    }
}