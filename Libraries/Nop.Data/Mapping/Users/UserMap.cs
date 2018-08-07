using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Users;

namespace Nop.Data.Mapping.Users
{
    /// <summary>
    /// Represents a customer mapping configuration
    /// </summary>
    public partial class UserMap : NopEntityTypeConfiguration<User>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Username).HasMaxLength(1000);
            builder.Property(customer => customer.Email).HasMaxLength(1000);
            builder.Property(customer => customer.EmailToRevalidate).HasMaxLength(1000);
            builder.Property(customer => customer.SystemName).HasMaxLength(400);

            builder.Ignore(customer => customer.UserRoles);

            base.Configure(builder);
        }

        #endregion
    }
}