using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial interface IBaseAdminModelFactory
    {
        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        /// <param name="items">Activity log type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareActivityLogTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareCountries(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareStatesAndProvinces(IList<SelectListItem> items, int? countryId,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareLanguages(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available user roles
        /// </summary>
        /// <param name="items">User role items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareUserRoles(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareEmailAccounts(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available time zones
        /// </summary>
        /// <param name="items">Time zone items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareTimeZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available log levels
        /// </summary>
        /// <param name="items">Log level items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareLogLevels(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available load plugin modes
        /// </summary>
        /// <param name="items">Load plugin mode items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareLoadPluginModes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available plugin groups
        /// </summary>
        /// <param name="items">Plugin group items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PreparePluginGroups(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available return request statuses
        /// </summary>
        /// <param name="items">Return request status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareReturnRequestStatuses(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available warehouses
        /// </summary>
        /// <param name="items">Warehouse items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareWarehouses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available delivery dates
        /// </summary>
        /// <param name="items">Delivery date items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        void PrepareDeliveryDates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
}