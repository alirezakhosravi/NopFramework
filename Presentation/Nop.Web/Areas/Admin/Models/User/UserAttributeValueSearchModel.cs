using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value search model
    /// </summary>
    public partial class UserAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserAttributeId { get; set; }

        #endregion
    }
}