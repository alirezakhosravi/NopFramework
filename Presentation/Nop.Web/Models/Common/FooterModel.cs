using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class FooterModel : BaseNopModel
    {
        public string StoreName { get; set; }
        public bool SitemapEnabled { get; set; }

        public int WorkingLanguageId { get; set; }

        public bool DisplaySitemapFooterItem { get; set; }
        public bool DisplayContactUsFooterItem { get; set; }
        public bool DisplayUserInfoFooterItem { get; set; }
        public bool DisplayUserAddressesFooterItem { get; set; }
    }
}