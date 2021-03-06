﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a general and common settings model
    /// </summary>
    public partial class GeneralCommonSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public GeneralCommonSettingsModel()
        {
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            FullTextSettings = new FullTextSettingsModel();
            DisplayDefaultMenuItemSettings = new DisplayDefaultMenuItemSettingsModel();
            DisplayDefaultFooterItemSettings = new DisplayDefaultFooterItemSettingsModel();
            AdminAreaSettings = new AdminAreaSettingsModel();
            SiteInformationSettings = new SiteInformationSettingsModel();
        }

        #endregion

        #region Properties

        public SeoSettingsModel SeoSettings { get; set; }

        public SecuritySettingsModel SecuritySettings { get; set; }

        public CaptchaSettingsModel CaptchaSettings { get; set; }

        public PdfSettingsModel PdfSettings { get; set; }

        public LocalizationSettingsModel LocalizationSettings { get; set; }

        public FullTextSettingsModel FullTextSettings { get; set; }

        public DisplayDefaultMenuItemSettingsModel DisplayDefaultMenuItemSettings { get; set; }

        public DisplayDefaultFooterItemSettingsModel DisplayDefaultFooterItemSettings { get; set; }

        public AdminAreaSettingsModel AdminAreaSettings { get; set; }

        public SiteInformationSettingsModel SiteInformationSettings { get; set; }
        #endregion
    }
}