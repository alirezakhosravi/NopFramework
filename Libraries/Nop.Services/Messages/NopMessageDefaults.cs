namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents default values related to messages services
    /// </summary>
    public static partial class NopMessageDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string MessageTemplatesAllCacheKey => "Nop.messagetemplate.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        public static string MessageTemplatesByNameCacheKey => "Nop.messagetemplate.name";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string MessageTemplatesPatternCacheKey => "Nop.messagetemplate.";
    }
}