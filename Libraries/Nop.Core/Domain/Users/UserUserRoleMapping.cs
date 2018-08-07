namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// Represents a User-User role mapping class
    /// </summary>
    public partial class UserUserRoleMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the User identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the User role identifier
        /// </summary>
        public int UserRoleId { get; set; }

        /// <summary>
        /// Gets or sets the User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the User role
        /// </summary>
        public virtual UserRole UserRole { get; set; }
    }
}