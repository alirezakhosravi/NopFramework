
//namespace Nop.Core.Domain.Notifications
//{
//    public partial class NotificationInfo : BaseEntity
//    {
//        public NotificationInfo()
//        {
//            Severity = NotificationSeverity.Info;
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
//        /// </summary>
//        public NotificationInfo(int id)
//        {
//            Id = id;
//        }

//        /// <summary>
//        /// Maximum length of <see cref="NotificationName"/> property.
//        /// Value: 96.
//        /// </summary>
//        public const int MaxNotificationNameLength = 96;

//        /// <summary>
//        /// Maximum length of <see cref="Data"/> property.
//        /// Value: 1048576 (1 MB).
//        /// </summary>
//        public const int MaxDataLength = 1024 * 1024;

//        /// <summary>
//        /// Maximum length of <see cref="DataTypeName"/> property.
//        /// Value: 512.
//        /// </summary>
//        public const int MaxDataTypeNameLength = 512;

//        /// <summary>
//        /// Maximum length of <see cref="EntityTypeName"/> property.
//        /// Value: 250.
//        /// </summary>
//        public const int MaxEntityTypeNameLength = 250;

//        /// <summary>
//        /// Maximum length of <see cref="EntityTypeAssemblyQualifiedName"/> property.
//        /// Value: 512.
//        /// </summary>
//        public const int MaxEntityTypeAssemblyQualifiedNameLength = 512;

//        /// <summary>
//        /// Maximum length of <see cref="EntityId"/> property.
//        /// Value: 96.
//        /// </summary>
//        public const int MaxEntityIdLength = 96;

//        /// <summary>
//        /// Maximum length of <see cref="UserIds"/> property.
//        /// Value: 131072 (128 KB).
//        /// </summary>
//        public const int MaxUserIdsLength = 128 * 1024;

//        /// <summary>
//        /// Unique notification name.
//        /// </summary>
//        public string NotificationName { get; set; }

//        /// <summary>
//        /// Notification data as JSON string.
//        /// </summary>
//        public string Data { get; set; }

//        /// <summary>
//        /// Type of the JSON serialized <see cref="Data"/>.
//        /// It's AssemblyQualifiedName of the type.
//        /// </summary>
//        public string DataTypeName { get; set; }

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

//        /// <summary>
//        /// Notification severity.
//        /// </summary>
//        public NotificationSeverity Severity { get; set; }

//        /// <summary>
//        /// Target users of the notification.
//        /// If this is set, it overrides subscribed users.
//        /// If this is null/empty, then notification is sent to all subscribed users.
//        /// </summary>
//        public string UserIds { get; set; }

//        /// <summary>
//        /// Excluded users.
//        /// This can be set to exclude some users while publishing notifications to subscribed users.
//        /// It's not normally used if <see cref="UserIds"/> is not null.
//        /// </summary>
//        public int ExcludedUserIds { get; set; }

//    }
//}
