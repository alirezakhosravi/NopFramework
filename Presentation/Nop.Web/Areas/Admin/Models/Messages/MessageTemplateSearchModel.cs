using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template search model
    /// </summary>
    public partial class MessageTemplateSearchModel : BaseSearchModel
    {
        #region Ctor

        public MessageTemplateSearchModel()
        {
            AvailableSites = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchSite")]
        public int SearchSiteId { get; set; }
        public IList<SelectListItem> AvailableSites { get; set; }

        #endregion
    }
}