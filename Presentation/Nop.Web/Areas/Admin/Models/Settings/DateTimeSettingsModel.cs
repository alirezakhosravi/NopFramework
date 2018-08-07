using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a date time settings model
    /// </summary>
    public partial class DateTimeSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public DateTimeSettingsModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToSetTimeZone")]
        public bool AllowUsersToSetTimeZone { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultTimeZone")]
        public string DefaultTimeZoneId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultTimeZone")]
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        #endregion
    }
}