using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a store information settings model
    /// </summary>
    public partial class SiteInformationSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public SiteInformationSettingsModel()
        {
            this.AvailableSiteThemes = new List<ThemeModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultSiteTheme")]
        public string DefaultSiteTheme { get; set; }
        public IList<ThemeModel> AvailableSiteThemes { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowUserToSelectTheme")]
        public bool AllowUserToSelectTheme { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Logo")]
        public int LogoPictureId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
        public bool DisplayEuCookieLawWarning { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
        public string FacebookLink { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
        public string TwitterLink { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
        public string YoutubeLink { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GooglePlusLink")]
        public string GooglePlusLink { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
        public bool SubjectFieldOnContactUsForm { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
        public bool UseSystemEmailForContactUsForm { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PopupForTermsOfServiceLinks")]
        public bool PopupForTermsOfServiceLinks { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
        public bool SitemapEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapPageSize")]
        public int SitemapPageSize { get; set; }

        #endregion

        #region Nested classes

        public partial class ThemeModel
        {
            public string SystemName { get; set; }
            public string FriendlyName { get; set; }
            public string PreviewImageUrl { get; set; }
            public string PreviewText { get; set; }
            public bool SupportRtl { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}