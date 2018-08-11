using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    /// <summary>
    /// Site information settings
    /// </summary>
    public class SiteInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets a default site theme
        /// </summary>
        public string DefaultSiteTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Users are allowed to select a theme
        /// </summary>
        public bool AllowUserToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public site (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInSite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed only for users with access to the admin area
        /// </summary>
        public bool DisplayMiniProfilerForAdminOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Google+ page URL of the site
        /// </summary>
        public string GooglePlusLink { get; set; }
    }
}
