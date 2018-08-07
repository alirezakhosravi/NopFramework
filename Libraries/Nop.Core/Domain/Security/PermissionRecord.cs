using System.Collections.Generic;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public partial class PermissionRecord : BaseEntity
    {
        private ICollection<PermissionRecordUserRoleMapping> _permissionRecordUserRoleMappings;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the permission record-User role mappings
        /// </summary>
        public virtual ICollection<PermissionRecordUserRoleMapping> PermissionRecordUserRoleMappings
        {
            get => _permissionRecordUserRoleMappings ?? (_permissionRecordUserRoleMappings = new List<PermissionRecordUserRoleMapping>());
            protected set => _permissionRecordUserRoleMappings = value;
        }   
    }
}