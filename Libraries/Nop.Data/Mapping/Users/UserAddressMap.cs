using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Users;

namespace Nop.Data.Mapping.Users
{
    /// <summary>
    /// Represents a customer-address mapping configuration
    /// </summary>
    public partial class UserAddressMap : NopEntityTypeConfiguration<UserAddressMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<UserAddressMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.UserAddressesTable);
            builder.HasKey(mapping => new { mapping.UserId, mapping.AddressId });

            builder.Property(mapping => mapping.UserId).HasColumnName("User_Id");
            builder.Property(mapping => mapping.AddressId).HasColumnName("Address_Id");

            builder.HasOne(mapping => mapping.User)
                .WithMany(customer => customer.UserAddressMappings)
                .HasForeignKey(mapping => mapping.UserId)
                .IsRequired();

            builder.HasOne(mapping => mapping.Address)
                .WithMany()
                .HasForeignKey(mapping => mapping.AddressId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}