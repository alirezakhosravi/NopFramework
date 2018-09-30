using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Notification;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a queued notification mapping configuration
    /// </summary>
    public partial class QueuedNotificationMap : NopEntityTypeConfiguration<QueuedNotification>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<QueuedNotification> builder)
        {
            builder.ToTable(nameof(QueuedNotification));
            builder.HasKey(email => email.Id);

            builder.Property(email => email.Subject).HasMaxLength(1000);

            builder.Ignore(email => email.Priority);
            builder.Ignore(email => email.ListOfUserIds);

            base.Configure(builder);
        }

        #endregion
    }
}