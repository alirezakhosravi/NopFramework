namespace Nop.Core.Domain.Users
{
    /// <summary>
    /// User registered event
    /// </summary>
    public class UserRegisteredEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="User">User</param>
        public UserRegisteredEvent(User User)
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