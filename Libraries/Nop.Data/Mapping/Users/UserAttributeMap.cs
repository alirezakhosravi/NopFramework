using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Users;

namespace Nop.Data.Mapping.Users
{
    /// <summary>
    /// Represents a customer attribute mapping configuration
    /// </summary>
    public partial class UserAttributeMap : NopEntityTypeConfiguration<UserAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<UserAttribute> builder)
        {
            builder.ToTable(nameof(UserAttribute));
            builder.HasKey(attribute => attribute.Id);

            builder.Property(attribute => attribute.Name).HasMaxLength(400).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}