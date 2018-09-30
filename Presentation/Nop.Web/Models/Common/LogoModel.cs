using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class LogoModel : BaseNopModel
    {
        public string SiteName { get; set; }

        public string LogoPath { get; set; }
    }
}