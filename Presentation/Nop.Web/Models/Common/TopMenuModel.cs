using System.Collections.Generic;
using System.Linq;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class TopMenuModel : BaseNopModel
    {
        public TopMenuModel()
        {
        }

        public bool DisplayHomePageMenuItem { get; set; }
        public bool DisplayUserInfoMenuItem { get; set; }
        public bool DisplayContactUsMenuItem { get; set; }
    }
}