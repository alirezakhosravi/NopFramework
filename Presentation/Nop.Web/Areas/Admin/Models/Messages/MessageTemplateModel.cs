using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template model
    /// </summary>
    public partial class MessageTemplateModel : BaseNopEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        #region Ctor

        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<SelectListItem>();

            SelectedSiteIds = new List<int>();
            AvailableSites = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }

        public int DelayPeriodId { get; set; }

        public bool HasAttachedDownload { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }

        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToSites")]
        public IList<int> SelectedSiteIds { get; set; }

        public IList<SelectListItem> AvailableSites { get; set; }

        //comma-separated list of stores used on the list page
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToSites")]
        public string ListOfSites { get; set; }

        public IList<MessageTemplateLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedLocaleModel
    {
        public MessageTemplateLocalizedModel()
        {
            AvailableEmailAccounts = new List<SelectListItem>();
        }

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        public string BccEmailAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }
    }
}