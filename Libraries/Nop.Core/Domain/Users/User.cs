using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// Represents a User
    /// </summary>
    public partial class User : BaseEntity
    {
        protected ICollection<UserAddressMapping> _userAddressMappings;
        private ICollection<UserUserRoleMapping> _userUserRoleMappings;
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private IList<UserRole> _userRoles;


        public User()
        {
            this.UserGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the User GUID
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the email that should be re-validated. Used in scenarios when a User is already registered and wants to change an email address.
        /// </summary>
        public string EmailToRevalidate { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User is required to re-login
        /// </summary>
        public bool RequireReLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of failed login attempts (wrong password)
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which a User cannot login (locked out)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User account is system
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the User system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        #region Navigation properties

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        {
            get => _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>());
            protected set => _externalAuthenticationRecords = value;
        }


        /// <summary>
        /// Gets or sets customer-customer role mappings
        /// </summary>
        public virtual ICollection<UserUserRoleMapping> UserUserRoleMappings
        {
            get => _userUserRoleMappings ?? (_userUserRoleMappings = new List<UserUserRoleMapping>());
            protected set => _userUserRoleMappings = value;
        }


        /// <summary>
        /// Gets or sets User roles
        /// </summary>
        public virtual IList<UserRole> UserRoles
        {
            get => _userRoles ?? (_userRoles = UserUserRoleMappings.Select(mapping => mapping.UserRole).ToList());
        }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public IList<Address> Addresses => UserAddressMappings.Select(mapping => mapping.Address).ToList();


        /// <summary>
        /// Gets or sets User-address mappings
        /// </summary>
        public virtual ICollection<UserAddressMapping> UserAddressMappings
        {
            get => _userAddressMappings ?? (_userAddressMappings = new List<UserAddressMapping>());
            protected set => _userAddressMappings = value;
        }

        #endregion
    }
}