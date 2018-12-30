using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents an address settings model
    /// </summary>
    public partial class AddressSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyEnabled")]
        public bool CompanyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyRequired")]
        public bool CompanyRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressEnabled")]
        public bool StreetAddressEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressRequired")]
        public bool StreetAddressRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Enabled")]
        public bool StreetAddress2Enabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Required")]
        public bool StreetAddress2Required { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeEnabled")]
        public bool ZipPostalCodeEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeRequired")]
        public bool ZipPostalCodeRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityEnabled")]
        public bool CityEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityRequired")]
        public bool CityRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyEnabled")]
        public bool CountyEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountyRequired")]
        public bool CountyRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountryEnabled")]
        public bool CountryEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StateProvinceEnabled")]
        public bool StateProvinceEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneEnabled")]
        public bool PhoneEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneRequired")]
        public bool PhoneRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxEnabled")]
        public bool FaxEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxRequired")]
        public bool FaxRequired { get; set; }

        #endregion
    }
}