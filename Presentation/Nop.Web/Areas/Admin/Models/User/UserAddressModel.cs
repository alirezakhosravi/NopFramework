using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user address model
    /// </summary>
    public partial class UserAddressModel : BaseNopModel
    {
        #region Ctor

        public UserAddressModel()
        {
            this.Address = new AddressModel();
        }

        #endregion

        #region Properties

        public int UserId { get; set; }

        public AddressModel Address { get; set; }

        #endregion
    }
}