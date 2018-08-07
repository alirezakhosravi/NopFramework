using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.User
{
    public partial class UserAddressEditModel : BaseNopModel
    {
        public UserAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        
        public AddressModel Address { get; set; }
    }
}