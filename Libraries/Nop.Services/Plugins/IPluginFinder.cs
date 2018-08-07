using System.Collections.Generic;
using Nop.Core.Domain.Users;
using Nop.Core.Plugins;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Plugin finder
    /// </summary>
    public interface IPluginFinder
    {
        /// <summary>
        /// Check whether the plugin is available
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <returns>true - available; false - no</returns>
        bool Authenticate(PluginDescriptor pluginDescriptor);

        /// <summary>
        /// Check that plugin is authorized for the specified user
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="user">user</param>
        /// <returns>True if authorized; otherwise, false</returns>
        bool AuthorizedForUser(PluginDescriptor pluginDescriptor, User user);

        /// <summary>
        /// Gets plugin groups
        /// </summary>
        /// <returns>Plugins groups</returns>
        IEnumerable<string> GetPluginGroups();

        /// <summary>
        /// Gets plugins
        /// </summary>
        /// <typeparam name="T">The type of plugins to get.</typeparam>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugins</returns>
        IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
                                     User user = null, string group = null) where T : class, IPlugin;

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugin descriptors</returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
                                                           User user = null, string group = null);

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugin descriptors</returns>
        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
                                                              User user = null, string group = null) where T : class, IPlugin;

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>>Plugin descriptor</returns>
        PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly);

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>>Plugin descriptor</returns>
        PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
            where T : class, IPlugin;

        /// <summary>
        /// Reload plugins after updating
        /// </summary>
        /// <param name="pluginDescriptor">Updated plugin descriptor</param>
        void ReloadPlugins(PluginDescriptor pluginDescriptor);
    }
}
