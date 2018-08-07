using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a customer user settings model
    /// </summary>
    public partial class UserUserSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public UserUserSettingsModel()
        {
            UserSettings = new UserSettingsModel();
            AddressSettings = new AddressSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
            ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
            UserAttributeSearchModel = new UserAttributeSearchModel();
            AddressAttributeSearchModel = new AddressAttributeSearchModel();
        }

        #endregion

        #region Properties

        public UserSettingsModel UserSettings { get; set; }

        public AddressSettingsModel AddressSettings { get; set; }

        public DateTimeSettingsModel DateTimeSettings { get; set; }

        public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

        public UserAttributeSearchModel UserAttributeSearchModel { get; set; }

        public AddressAttributeSearchModel AddressAttributeSearchModel { get; set; }

        #endregion
    }
}