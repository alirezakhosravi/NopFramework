//using System;
//namespace Nop.Core.Domain.Notifications
//{
//    /// <summary>
//    /// Used to store a user notification.
//    /// </summary>
//    public partial class UserNotificationInfo : BaseEntity
//    {
//        public UserNotificationInfo()
//        {

//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="UserNotificationInfo"/> class.
//        /// </summary>
//        /// <param name="id"></param>
//        public UserNotificationInfo(int id)
//        {
//            Id = id;
//            State = UserNotificationState.Unread;
//            CreationTime = DateTime.UtcNow;
//        }

//        /// <summary>
//        /// User Id.
//        /// </summary>
//        public long UserId { get; set; }

//        /// <summary>
//        /// Current state of the user notification.
//        /// </summary>
//        public UserNotificationState State { get; set; }

//        public DateTime CreationTime { get; set; }

//    }
//}
