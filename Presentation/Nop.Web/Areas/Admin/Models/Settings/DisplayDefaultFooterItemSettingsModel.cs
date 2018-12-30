using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a display default footer item settings model
    /// </summary>
    public partial class DisplayDefaultFooterItemSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem")]
        public bool DisplaySitemapFooterItem { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem")]
        public bool DisplayContactUsFooterItem { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserInfoFooterItem")]
        public bool DisplayUserInfoFooterItem { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserAddressesFooterItem")]
        public bool DisplayUserAddressesFooterItem { get; set; }

        #endregion
    }
}