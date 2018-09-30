using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Users;
using Nop.Services.Common;
using Nop.Services.Themes;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents the theme context implementation
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IThemeProvider _themeProvider;
        private readonly IWorkContext _workContext;
        private readonly SiteInformationSettings _siteInformationSettings;

        private string _cachedThemeName;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="themeProvider">Theme provider</param>
        /// <param name="workContext">Work context</param>
        /// <param name="siteInformationSettings">Site information settings</param>
        public ThemeContext(IGenericAttributeService genericAttributeService,
            IThemeProvider themeProvider,
            IWorkContext workContext,
            SiteInformationSettings siteInformationSettings)
        {
            this._genericAttributeService = genericAttributeService;
            this._themeProvider = themeProvider;
            this._workContext = workContext;
            this._siteInformationSettings = siteInformationSettings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedThemeName))
                    return _cachedThemeName;

                var themeName = string.Empty;

                //whether users are allowed to select a theme
                if (_siteInformationSettings.AllowUserToSelectTheme && _workContext.CurrentUser != null)
                {
                    themeName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentUser, NopUserDefaults.WorkingThemeNameAttribute);
                }

                //if not, try to get default site theme
                if (string.IsNullOrEmpty(themeName))
                    themeName = _siteInformationSettings.DefaultSiteTheme;

                //ensure that this theme exists
                if (!_themeProvider.ThemeExists(themeName))
                {
                    //if it does not exist, try to get the first one
                    themeName = _themeProvider.GetThemes().FirstOrDefault()?.SystemName
                        ?? throw new Exception("No theme could be loaded");
                }

                //cache theme system name
                this._cachedThemeName = themeName;

                return themeName;
            }
            set
            {
                //whether users are allowed to select a theme
                if (!_siteInformationSettings.AllowUserToSelectTheme || _workContext.CurrentUser == null)
                    return;

                //save selected by user theme system name
                _genericAttributeService.SaveAttribute(_workContext.CurrentUser,
                    NopUserDefaults.WorkingThemeNameAttribute, value);

                //clear cache
                this._cachedThemeName = null;
            }
        }

        #endregion
    }
}