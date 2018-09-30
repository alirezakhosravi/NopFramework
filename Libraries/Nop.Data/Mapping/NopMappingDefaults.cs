
namespace Nop.Data.Mapping
{
    /// <summary>
    /// Represents default values related to data mapping
    /// </summary>
    public static partial class NopMappingDefaults
    {
        /// <summary>
        /// Gets a name of the User-Addresses mapping table
        /// </summary>
        public static string UserAddressesTable => "UserAddresses";

        /// <summary>
        /// Gets a name of the User-UserRole mapping table
        /// </summary>
        public static string UserUserRoleTable => "User_UserRole_Mapping";

        /// <summary>
        /// Gets a name of the PermissionRecord-UserRole mapping table
        /// </summary>
        public static string PermissionRecordRoleTable => "PermissionRecord_Role_Mapping";
    }
}