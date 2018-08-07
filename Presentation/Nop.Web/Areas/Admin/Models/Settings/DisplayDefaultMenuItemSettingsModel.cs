using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a display default menu item settings model
    /// </summary>
    public partial class DisplayDefaultMenuItemSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem")]
        public bool DisplayHomePageMenuItem { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem")]
        public bool DisplayUserInfoMenuItem { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
        public bool DisplayContactUsMenuItem { get; set; }

        #endregion
    }
}