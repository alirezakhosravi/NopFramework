using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a PDF settings model
    /// </summary>
    public partial class PdfSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
        public bool LetterPageSizeEnabled { get; set; }
        public bool LetterPageSizeEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
        [UIHint("Picture")]
        public int LogoPictureId { get; set; }
        public bool LogoPictureId_OverrideForStore { get; set; }

        #endregion
    }
}