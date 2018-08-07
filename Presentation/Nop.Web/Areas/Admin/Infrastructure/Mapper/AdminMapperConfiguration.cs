using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public AdminMapperConfiguration()
        {
            //create specific maps
            CreateAuthenticationMaps();
            CreateCmsMaps();
            CreateCommonMaps();
            CreateUsersMaps();
            CreateDirectoryMaps();
            CreateLocalizationMaps();
            CreateLoggingMaps();
            CreateMediaMaps();
            CreateMessagesMaps();
            CreatePluginsMaps();
            CreateSecurityMaps();
            CreateSeoMaps();
            CreateTasksMaps();

            //add some generic mapping rules
            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseNopModel
                if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                }

                //exclude Locales from mapping ILocalizedModel
                if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());
                
                //exclude some properties from mapping ACL supported entities and models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableUserRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedUserRoleIds), options => options.Ignore());
                }

                if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //exclude some properties from mapping plugin models
                    map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
                    map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

                    //define specific rules for mapping plugin models
                    if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
                    {
                        map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                        map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                        map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
                    }
                }
            });
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create authentication maps 
        /// </summary>
        protected virtual void CreateAuthenticationMaps()
        {
            CreateMap<IExternalAuthenticationMethod, ExternalAuthenticationMethodModel>();
        }

        /// <summary>
        /// Create CMS maps 
        /// </summary>
        protected virtual void CreateCmsMaps()
        {
            CreateMap<IWidgetPlugin, WidgetModel>()
                .ForMember(model => model.WidgetViewComponentArguments, options => options.Ignore())
                .ForMember(model => model.WidgetViewComponentName, options => options.Ignore());
        }

        /// <summary>
        /// Create common maps 
        /// </summary>
        protected virtual void CreateCommonMaps()
        {
            CreateMap<Address, AddressModel>()
                .ForMember(model => model.AddressHtml, options => options.Ignore())
                .ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.AvailableStates, options => options.Ignore())
                .ForMember(model => model.CityEnabled, options => options.Ignore())
                .ForMember(model => model.CityRequired, options => options.Ignore())
                .ForMember(model => model.CompanyEnabled, options => options.Ignore())
                .ForMember(model => model.CompanyRequired, options => options.Ignore())
                .ForMember(model => model.CountryEnabled, options => options.Ignore())
                .ForMember(model => model.CountryName, options => options.MapFrom(entity => entity.Country != null ? entity.Country.Name : null))
                .ForMember(model => model.CountryRequired, options => options.Ignore())
                .ForMember(model => model.CountyEnabled, options => options.Ignore())
                .ForMember(model => model.CountyRequired, options => options.Ignore())
                .ForMember(model => model.CustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.EmailEnabled, options => options.Ignore())
                .ForMember(model => model.EmailRequired, options => options.Ignore())
                .ForMember(model => model.FaxEnabled, options => options.Ignore())
                .ForMember(model => model.FaxRequired, options => options.Ignore())
                .ForMember(model => model.FirstNameEnabled, options => options.Ignore())
                .ForMember(model => model.FirstNameRequired, options => options.Ignore())
                .ForMember(model => model.FormattedCustomAddressAttributes, options => options.Ignore())
                .ForMember(model => model.LastNameEnabled, options => options.Ignore())
                .ForMember(model => model.LastNameRequired, options => options.Ignore())
                .ForMember(model => model.PhoneEnabled, options => options.Ignore())
                .ForMember(model => model.PhoneRequired, options => options.Ignore())
                .ForMember(model => model.StateProvinceEnabled, options => options.Ignore())
                .ForMember(model => model.StateProvinceName, options => options.MapFrom(entity => entity.StateProvince != null ? entity.StateProvince.Name : null))
                .ForMember(model => model.StreetAddress2Enabled, options => options.Ignore())
                .ForMember(model => model.StreetAddress2Required, options => options.Ignore())
                .ForMember(model => model.StreetAddressEnabled, options => options.Ignore())
                .ForMember(model => model.StreetAddressRequired, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeEnabled, options => options.Ignore())
                .ForMember(model => model.ZipPostalCodeRequired, options => options.Ignore());
            CreateMap<AddressModel, Address>()
                .ForMember(entity => entity.Country, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CustomAttributes, options => options.Ignore())
                .ForMember(entity => entity.StateProvince, options => options.Ignore());

            CreateMap<AddressAttribute, AddressAttributeModel>()
                .ForMember(model => model.AddressAttributeValueSearchModel, options => options.Ignore())
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore());
            
            CreateMap<AddressAttributeValue, AddressAttributeValueModel>();
            CreateMap<AddressAttributeValueModel, AddressAttributeValue>()
                .ForMember(entity => entity.AddressAttribute, options => options.Ignore());

            CreateMap<AddressSettings, AddressSettingsModel>();
            CreateMap<AddressSettingsModel, AddressSettings>()
                .ForMember(settings => settings.PreselectCountryIfOnlyOne, options => options.Ignore());
        }

        /// <summary>
        /// Create users maps 
        /// </summary>
        protected virtual void CreateUsersMaps()
        {
            CreateMap<UserAttribute, UserAttributeModel>()
                .ForMember(model => model.AttributeControlTypeName, options => options.Ignore())
                .ForMember(model => model.UserAttributeValueSearchModel, options => options.Ignore());
            
            CreateMap<UserAttributeValue, UserAttributeValueModel>();
            CreateMap<UserAttributeValueModel, UserAttributeValue>()
                .ForMember(entity => entity.UserAttribute, options => options.Ignore());

            CreateMap<UserRole, UserRoleModel>()
                .ForMember(model => model.PurchasedWithProductName, options => options.Ignore())
                .ForMember(model => model.TaxDisplayTypeValues, options => options.Ignore());
            CreateMap<UserRoleModel, UserRole>()
                .ForMember(entity => entity.PermissionRecordUserRoleMappings, options => options.Ignore());

            CreateMap<UserSettings, UserSettingsModel>();
            CreateMap<UserSettingsModel, UserSettings>()
                .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
                .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
                .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
                .ForMember(settings => settings.OnlineUserMinutes, options => options.Ignore())
                .ForMember(settings => settings.SuffixDeletedUsers, options => options.Ignore());
        }

        /// <summary>
        /// Create directory maps 
        /// </summary>
        protected virtual void CreateDirectoryMaps()
        {
            CreateMap<Country, CountryModel>()
                .ForMember(model => model.StateProvinceSearchModel, options => options.Ignore());
            
            CreateMap<StateProvince, StateProvinceModel>();
            CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(entity => entity.Country, options => options.Ignore());
        }

        /// <summary>
        /// Create localization maps 
        /// </summary>
        protected virtual void CreateLocalizationMaps()
        {
            CreateMap<Language, LanguageModel>()
                .ForMember(model => model.LocaleResourceSearchModel, options => options.Ignore());
            CreateMap<LanguageModel, Language>();
        }

        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.MapFrom(entity => entity.ActivityLogType.Name))
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.MapFrom(entity => entity.User.Email));
            CreateMap<ActivityLogModel, ActivityLog>()
                .ForMember(entity => entity.ActivityLogType, options => options.Ignore())
                .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.User, options => options.Ignore())
                .ForMember(entity => entity.EntityId, options => options.Ignore())
                .ForMember(entity => entity.EntityName, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.User, options => options.Ignore())
                .ForMember(entity => entity.LogLevelId, options => options.Ignore());
        }

        /// <summary>
        /// Create media maps 
        /// </summary>
        protected virtual void CreateMediaMaps()
        {
            CreateMap<MediaSettings, MediaSettingsModel>()
                .ForMember(model => model.PicturesStoredIntoDatabase, options => options.Ignore());
            CreateMap<MediaSettingsModel, MediaSettings>()
                .ForMember(settings => settings.AzureCacheControlHeader, options => options.Ignore())
                .ForMember(settings => settings.ImageSquarePictureSize, options => options.Ignore());
        }

        /// <summary>
        /// Create messages maps 
        /// </summary>
        protected virtual void CreateMessagesMaps()
        {
            CreateMap<Campaign, CampaignModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableUserRoles, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountId, options => options.Ignore())
                .ForMember(model => model.TestEmail, options => options.Ignore());
            CreateMap<CampaignModel, Campaign>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore());

            CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(model => model.IsDefaultEmailAccount, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.SendTestEmailTo, options => options.Ignore());
            CreateMap<EmailAccountModel, EmailAccount>()
                .ForMember(entity => entity.Password, options => options.Ignore());

            CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(model => model.AllowedTokens, options => options.Ignore())
                .ForMember(model => model.AvailableEmailAccounts, options => options.Ignore())
                .ForMember(model => model.HasAttachedDownload, options => options.Ignore())
                .ForMember(model => model.ListOfStores, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore());
            CreateMap<MessageTemplateModel, MessageTemplate>()
                .ForMember(entity => entity.DelayPeriod, options => options.Ignore());

            CreateMap<NewsLetterSubscription, NewsletterSubscriptionModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<NewsletterSubscriptionModel, NewsLetterSubscription>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.NewsLetterSubscriptionGuid, options => options.Ignore());

            CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.EmailAccountName, options => options.MapFrom(entity => entity.EmailAccount != null ? entity.EmailAccount.FriendlyName : string.Empty))
                .ForMember(model => model.PriorityName, options => options.Ignore())
                .ForMember(model => model.SendImmediately, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
                .ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailAccount, options => options.Ignore())
                .ForMember(entity => entity.EmailAccountId, options => options.Ignore())
                .ForMember(entity => entity.Priority, options => options.Ignore())
                .ForMember(entity => entity.PriorityId, options => options.Ignore())
                .ForMember(entity => entity.SentOnUtc, options => options.Ignore());
        }

        /// <summary>
        /// Create plugins maps 
        /// </summary>
        protected virtual void CreatePluginsMaps()
        {
            CreateMap<PluginDescriptor, PluginModel>()
                .ForMember(model => model.CanChangeEnabled, options => options.Ignore())
                .ForMember(model => model.IsEnabled, options => options.Ignore());
        }

        /// <summary>
        /// Create security maps 
        /// </summary>
        protected virtual void CreateSecurityMaps()
        {
            CreateMap<CaptchaSettings, CaptchaSettingsModel>()
                .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPrivateKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ReCaptchaPublicKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnApplyVendorPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnBlogCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnContactUsPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailProductToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnEmailWishlistToFriendPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnLoginPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnNewsCommentPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnProductReviewPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowOnRegistrationPage_OverrideForStore, options => options.Ignore());
            CreateMap<CaptchaSettingsModel, CaptchaSettings>()
                .ForMember(settings => settings.AutomaticallyChooseLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaDefaultLanguage, options => options.Ignore())
                .ForMember(settings => settings.ReCaptchaTheme, options => options.Ignore());
        }

        /// <summary>
        /// Create SEO maps 
        /// </summary>
        protected virtual void CreateSeoMaps()
        {
            CreateMap<UrlRecord, UrlRecordModel>()
                .ForMember(model => model.DetailsUrl, options => options.Ignore())
                .ForMember(model => model.Language, options => options.Ignore())
                .ForMember(model => model.Name, options => options.Ignore());
            CreateMap<UrlRecordModel, UrlRecord>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.Slug, options => options.Ignore());
        }

        /// <summary>
        /// Create tasks maps 
        /// </summary>
        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.Type, options => options.Ignore());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
    }
}