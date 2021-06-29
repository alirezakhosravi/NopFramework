using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user model
    /// </summary>
    public partial class UserModel : BaseNopEntityModel, IAclSupportedModel
    {
        #region Ctor

        public UserModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.SendEmail = new SendEmailModel() { SendImmediately = true };
            this.SendPm = new SendPmModel();

            this.SelectedUserRoleIds = new List<int>();
            this.AvailableUserRoles = new List<SelectListItem>();

            this.AssociatedExternalAuthRecords = new List<UserAssociatedExternalAuthModel>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.UserAttributes = new List<UserAttributeModel>();
            this.UserAddressSearchModel = new UserAddressSearchModel();
            this.UserActivityLogSearchModel = new UserActivityLogSearchModel();
        }

        #endregion

        #region Properties

        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Username")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.Users.Users.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Password")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.FirstName")]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.LastName")]
        public string LastName { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.FullName")]
        public string FullName { get; set; }

        public bool DateOfBirthEnabled { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Users.Users.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Company")]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.City")]
        public string City { get; set; }

        public bool CountyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.County")]
        public string County { get; set; }

        public bool CountryEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.StateProvince")]
        public int StateProvinceId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Admin.Users.Users.Fields.Phone")]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }

        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Admin.Users.Users.Fields.Fax")]
        public string Fax { get; set; }

        public List<UserAttributeModel> UserAttributes { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.RegisteredInStore")]
        public string RegisteredInStore { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public int AffiliateId { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //time zone
        [NopResourceDisplayName("Admin.Users.Users.Fields.TimeZoneId")]
        public string TimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Admin.Users.Users.Fields.VatNumber")]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }

        //registration date
        [NopResourceDisplayName("Admin.Users.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP address
        [NopResourceDisplayName("Admin.Users.Users.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }

        //user roles
        [NopResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public string UserRoleNames { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }

        //send PM model
        public SendPmModel SendPm { get; set; }

        //send the welcome message
        public bool AllowSendingOfWelcomeMessage { get; set; }

        //re-send the activation message
        public bool AllowReSendingOfActivationMessage { get; set; }

        [NopResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
        public IList<UserAssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }

        public string AvatarUrl { get; internal set; }

        public UserAddressSearchModel UserAddressSearchModel { get; set; }

        public UserActivityLogSearchModel UserActivityLogSearchModel { get; set; }

        #endregion

        #region Nested classes

        public partial class SendEmailModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Users.Users.SendEmail.Subject")]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Users.Users.SendEmail.Body")]
            public string Body { get; set; }

            [NopResourceDisplayName("Admin.Users.Users.SendEmail.SendImmediately")]
            public bool SendImmediately { get; set; }

            [NopResourceDisplayName("Admin.Users.Users.SendEmail.DontSendBeforeDate")]
            [UIHint("DateTimeNullable")]
            public DateTime? DontSendBeforeDate { get; set; }
        }

        public partial class SendPmModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Users.Users.SendPM.Subject")]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Users.Users.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class UserAttributeModel : BaseNopEntityModel
        {
            public UserAttributeModel()
            {
                Values = new List<UserAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }


            public IList<UserAttributeValueModel> Values { get; set; }
        }

        public partial class UserAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}