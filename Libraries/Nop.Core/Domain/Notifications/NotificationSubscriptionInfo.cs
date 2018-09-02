//using System;

//namespace Nop.Core.Domain.Notifications
//{
//    /// <summary>
//    /// Used to store a notification subscription.
//    /// </summary>
//    public class NotificationSubscriptionInfo : BaseEntity
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="NotificationSubscriptionInfo"/> class.
//        /// </summary>
//        public NotificationSubscriptionInfo()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="NotificationSubscriptionInfo"/> class.
//        /// </summary>
//        public NotificationSubscriptionInfo(int id, int userId, string notificationName, BaseEntity entityIdentifier = null) : this()
//        {
//            Id = id;
//            NotificationName = notificationName;
//            UserId = userId;
//            EntityTypeName = entityIdentifier == null ? null : entityIdentifier.EntityType.FullName;
//            EntityTypeAssemblyQualifiedName = entityIdentifier == null ? null : entityIdentifier.EntityType.AssemblyQualifiedName;
//            EntityId = entityIdentifier == null ? null : entityIdentifier.Id.ToString();
//        }

//        /// <summary>
//        /// User Id.
//        /// </summary>
//        public long UserId { get; set; }

//        /// <summary>
//        /// Notification unique name.
//        /// </summary>
//        public string NotificationName { get; set; }

//        /// <summary>
//        /// Gets/sets entity type name, if this is an entity level notification.
//        /// It's FullName of the entity type.
//        /// </summary>
//        public string EntityTypeName { get; set; }

//        /// <summary>
//        /// AssemblyQualifiedName of the entity type.
//        /// </summary>
//        public string EntityTypeAssemblyQualifiedName { get; set; }

//        /// <summary>
//        /// Gets/sets primary key of the entity, if this is an entity level notification.
//        /// </summary>
//        public string EntityId { get; set; }

//    }
//}
