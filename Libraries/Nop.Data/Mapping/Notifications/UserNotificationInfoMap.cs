//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Nop.Core.Domain.Notifications;

//namespace Nop.Data.Mapping.Notifications
//{
//    /// <summary>
//    /// Represents an notification subscription info mapping configuration
//    /// </summary>
//    public partial class UserNotificationInfoMap : NopEntityTypeConfiguration<UserNotificationInfo>
//    {
//        #region Methods

//        /// <summary>
//        /// Configures the entity
//        /// </summary>
//        /// <param name="builder">The builder to be used to configure the entity</param>
//        public override void Configure(EntityTypeBuilder<UserNotificationInfo> builder)
//        {
//            builder.ToTable(nameof(NotificationInfo));
//            builder.HasKey(record => record.Id);

//            base.Configure(builder);
//        }

//        #endregion
//    }
//}