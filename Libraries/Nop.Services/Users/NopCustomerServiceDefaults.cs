namespace Nop.Services.Users
{
    /// <summary>
    /// Represents default values related to user services
    /// </summary>
    public static partial class NopUserServiceDefaults
    {
        #region User attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string UserAttributesAllCacheKey => "Nop.userattribute.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute ID
        /// </remarks>
        public static string UserAttributesByIdCacheKey => "Nop.userattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute ID
        /// </remarks>
        public static string UserAttributeValuesAllCacheKey => "Nop.userattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user attribute value ID
        /// </remarks>
        public static string UserAttributeValuesByIdCacheKey => "Nop.userattributevalue.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserAttributesPatternCacheKey => "Nop.userattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserAttributeValuesPatternCacheKey => "Nop.userattributevalue.";

        #endregion

        #region User roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static string UserRolesAllCacheKey => "Nop.userrole.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static string UserRolesBySystemNameCacheKey => "Nop.userrole.systemname-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UserRolesPatternCacheKey => "Nop.userrole.";

        #endregion

        /// <summary>
        /// Gets a key for caching current user password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : user identifier
        /// </remarks>
        public static string UserPasswordLifetimeCacheKey => "Nop.users.passwordlifetime-{0}";

        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;
        
        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int UserUsernameLength => 100;
    }
}