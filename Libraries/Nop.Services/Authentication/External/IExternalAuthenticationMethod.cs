using Nop.Core.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents method for the external authentication
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
        /// <summary>
        /// Gets a name of a view component for displaying plugin in public Site
        /// </summary>
        /// <returns>View component name</returns>
        string GetPublicViewComponentName();
    }
}
