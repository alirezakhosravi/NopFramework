using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Factories;
using Nop.Web.Infrastructure.Installation;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();

            //common factories
            builder.RegisterType<AclSupportedModelFactory>().As<IAclSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizedModelFactory>().As<ILocalizedModelFactory>().InstancePerLifetimeScope();

            //admin factories
            builder.RegisterType<BaseAdminModelFactory>().As<IBaseAdminModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityLogModelFactory>().As<IActivityLogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AddressAttributeModelFactory>().As<IAddressAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CountryModelFactory>().As<ICountryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserAttributeModelFactory>().As<IUserAttributeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserModelFactory>().As<IUserModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserRoleModelFactory>().As<IUserRoleModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<EmailAccountModelFactory>().As<IEmailAccountModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalAuthenticationMethodModelFactory>().As<IExternalAuthenticationMethodModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<HomeModelFactory>().As<IHomeModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageModelFactory>().As<ILanguageModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LogModelFactory>().As<ILogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTemplateModelFactory>().As<IMessageTemplateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PluginModelFactory>().As<IPluginModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReportModelFactory>().As<IReportModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailModelFactory>().As<IQueuedEmailModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleTaskModelFactory>().As<IScheduleTaskModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityModelFactory>().As<ISecurityModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SettingModelFactory>().As<ISettingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateModelFactory>().As<ITemplateModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetModelFactory>().As<IWidgetModelFactory>().InstancePerLifetimeScope();

            //factories
            builder.RegisterType<Factories.AddressModelFactory>().As<Factories.IAddressModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CommonModelFactory>().As<Factories.ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.CountryModelFactory>().As<Factories.ICountryModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.UserModelFactory>().As<Factories.IUserModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ExternalAuthenticationModelFactory>().As<Factories.IExternalAuthenticationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.ProfileModelFactory>().As<Factories.IProfileModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.WidgetModelFactory>().As<Factories.IWidgetModelFactory>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
