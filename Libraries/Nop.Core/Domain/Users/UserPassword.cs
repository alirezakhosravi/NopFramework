using System;

namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// Represents a User password
    /// </summary>
    public partial class UserPassword : BaseEntity
    {
        public UserPassword()
        {
            this.PasswordFormat = PasswordFormat.Clear;
        }

        /// <summary>
        /// Gets or sets the User identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password format identifier
        /// </summary>
        public int PasswordFormatId { get; set; }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        public PasswordFormat PasswordFormat
        {
            get => (PasswordFormat)PasswordFormatId;
            set => PasswordFormatId = (int)value;
        }

        /// <summary>
        /// Gets or sets the User
        /// </summary>
        public virtual User User { get; set; }
    }
}