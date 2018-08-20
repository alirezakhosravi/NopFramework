using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Search
{
    public class SearchModel : BaseNopModel
    {
        #region Properties
        public string Title { get; set; }

        public string Description { get; set; }

        public string Route { get; set; }
        #endregion
    }
}
