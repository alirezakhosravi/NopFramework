//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Nop.Core.Domain.Notifications;

//namespace Nop.Data.Mapping.Notifications
//{
//    /// <summary>
//    /// Represents an notification info mapping configuration
//    /// </summary>
//    public partial class NotificationInfoMap : NopEntityTypeConfiguration<NotificationInfo>
//    {
//        #region Methods

//        /// <summary>
//        /// Configures the entity
//        /// </summary>
//        /// <param name="builder">The builder to be used to configure the entity</param>
//        public override void Configure(EntityTypeBuilder<NotificationInfo> builder)
//        {
//            builder.ToTable(nameof(NotificationInfo));
//            builder.HasKey(record => record.Id);

//            builder.Property(record => record.NotificationName).HasMaxLength(NotificationInfo.MaxNotificationNameLength).IsRequired();

//            builder.Property(record => record.Data).HasMaxLength(NotificationInfo.MaxDataLength);

//            builder.Property(record => record.DataTypeName).HasMaxLength(NotificationInfo.MaxDataTypeNameLength);

//            builder.Property(record => record.EntityTypeName).HasMaxLength(NotificationInfo.MaxEntityTypeNameLength);

//            builder.Property(record => record.EntityTypeAssemblyQualifiedName).HasMaxLength(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength);

//            builder.Property(record => record.EntityId).HasMaxLength(NotificationInfo.MaxEntityIdLength);

//            base.Configure(builder);
//        }

//        #endregion
//    }
//}