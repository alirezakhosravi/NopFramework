using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Users;

namespace Nop.Data.Mapping.Users
{
    /// <summary>
    /// Represents a customer password mapping configuration
    /// </summary>
    public partial class UserPasswordMap : NopEntityTypeConfiguration<UserPassword>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<UserPassword> builder)
        {
            builder.ToTable(nameof(UserPassword));
            builder.HasKey(password => password.Id);

            builder.HasOne(password => password.User)
                .WithMany()
                .HasForeignKey(password => password.UserId)
                .IsRequired();

            builder.Ignore(password => password.PasswordFormat);

            base.Configure(builder);
        }

        #endregion
    }
}