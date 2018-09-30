using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Display default menu item settings
    /// </summary>
    public class DisplayDefaultMenuItemSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display "home page" menu item
        /// </summary>
        public bool DisplayHomePageMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "customer info" menu item
        /// </summary>
        public bool DisplayUserInfoMenuItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display "contact us" menu item
        /// </summary>
        public bool DisplayContactUsMenuItem { get; set; }
    }
}