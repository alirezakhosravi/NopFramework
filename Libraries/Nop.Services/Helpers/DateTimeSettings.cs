using Nop.Core.Configuration;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// DateTime settings
    /// </summary>
    public class DateTimeSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a default time zone identifier
        /// </summary>
        public string DefaultTimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select theirs time zone
        /// </summary>
        public bool AllowUsersToSetTimeZone { get; set; }
    }
}