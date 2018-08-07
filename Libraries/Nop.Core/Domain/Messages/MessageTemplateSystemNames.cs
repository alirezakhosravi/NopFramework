namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents message template system names
    /// </summary>
    public static partial class MessageTemplateSystemNames
    {
        #region User

        /// <summary>
        /// Represents system name of notification about new registration
        /// </summary>
        public const string UserRegisteredNotification = "NewUser.Notification";

        /// <summary>
        /// Represents system name of User welcome message
        /// </summary>
        public const string UserWelcomeMessage = "User.WelcomeMessage";

        /// <summary>
        /// Represents system name of email validation message
        /// </summary>
        public const string UserEmailValidationMessage = "User.EmailValidationMessage";

        /// <summary>
        /// Represents system name of email revalidation message
        /// </summary>
        public const string UserEmailRevalidationMessage = "User.EmailRevalidationMessage";

        /// <summary>
        /// Represents system name of password recovery message
        /// </summary>
        public const string UserPasswordRecoveryMessage = "User.PasswordRecovery";

        #endregion

        #region Misc

        /// <summary>
        /// Represents system name of 'Contact us' message
        /// </summary>
        public const string ContactUsMessage = "Service.ContactUs";

        #endregion
    }
}