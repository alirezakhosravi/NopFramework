using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class AddressAttributeModel : BaseNopEntityModel
    {
        public AddressAttributeModel()
        {
            Values = new List<AddressAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public IList<AddressAttributeValueModel> Values { get; set; }
    }

    public partial class AddressAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}