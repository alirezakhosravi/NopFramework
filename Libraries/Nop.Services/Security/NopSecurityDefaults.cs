namespace Nop.Services.Security
{
    /// <summary>
    /// Represents default values related to security services
    /// </summary>
    public static partial class NopSecurityDefaults
    {
        #region Access control list

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string AclRecordByEntityIdNameCacheKey => "Nop.aclrecord.entityid-name-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AclRecordPatternCacheKey => "Nop.aclrecord.";

        #endregion

        #region Permissions

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user role ID
        /// {1} : permission system name
        /// </remarks>
        public static string PermissionsAllowedCacheKey => "Nop.permission.allowed-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user role ID
        /// </remarks>
        public static string PermissionsAllByUserRoleIdCacheKey => "Nop.permission.allbyuserroleid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PermissionsPatternCacheKey => "Nop.permission.";

        #endregion
    }
}