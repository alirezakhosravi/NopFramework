using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a language model
    /// </summary>
    public partial class LanguageModel : BaseNopEntityModel
    {
        #region Ctor

        public LanguageModel()
        {
            LocaleResourceSearchModel = new LocaleResourceSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        public string LanguageCulture { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        public string UniqueSeoCode { get; set; }
        
        //flags
        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        public string FlagImageFileName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        // search
        public LocaleResourceSearchModel LocaleResourceSearchModel { get; set; }

        #endregion
    }
}