namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// User logged-in event
    /// </summary>
    public class UserLoggedinEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="User">User</param>
        public UserLoggedinEvent(User User)
        {
            this.User = User;
        }

        /// <summary>
        /// User
        /// </summary>
        public User User
        {
            get;
        }
    }
}