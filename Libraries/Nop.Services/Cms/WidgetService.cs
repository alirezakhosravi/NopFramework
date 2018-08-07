using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Users;
using Nop.Services.Plugins;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Widget service
    /// </summary>
    public partial class WidgetService : IWidgetService
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public WidgetService(IPluginFinder pluginFinder,
            WidgetSettings widgetSettings)
        {
            this._pluginFinder = pluginFinder;
            this._widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadActiveWidgets(User user = null)
        {
            return LoadAllWidgets(user)
                .Where(x => _widgetSettings.ActiveWidgetSystemNames.Contains(x.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone, User user = null)
        {
            if (string.IsNullOrWhiteSpace(widgetZone))
                return new List<IWidgetPlugin>();

            return LoadActiveWidgets(user)
                .Where(x => x.GetWidgetZones().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        public virtual IWidgetPlugin LoadWidgetBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            return descriptor?.Instance<IWidgetPlugin>();
        }

        /// <summary>
        /// Load all widgets
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadAllWidgets(User user = null)
        {
            return _pluginFinder.GetPlugins<IWidgetPlugin>(user: user).ToList();
        }

        /// <summary>
        /// Is widget active
        /// </summary>
        /// <param name="widget">Widget</param>
        /// <returns>Result</returns>
        public virtual bool IsWidgetActive(IWidgetPlugin widget)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            if (_widgetSettings.ActiveWidgetSystemNames == null)
                return false;

            foreach (var activeMethodSystemName in _widgetSettings.ActiveWidgetSystemNames)
                if (widget.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
        #endregion
    }
}