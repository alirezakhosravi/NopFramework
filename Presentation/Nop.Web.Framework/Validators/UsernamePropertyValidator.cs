using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Validators;
using Nop.Core.Domain.Users;

namespace Nop.Web.Framework.Validators
{
    /// <summary>
    /// Username validator
    /// </summary>
    public class UsernamePropertyValidator : PropertyValidator
    {
        private readonly UserSettings _userSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        public UsernamePropertyValidator(UserSettings userSettings)
            : base("Username is not valid")
        {
            this._userSettings = userSettings;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return IsValid(context.PropertyValue as string, _userSettings);
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="userSettings">User settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string username, UserSettings userSettings)
        {
            if (!userSettings.UsernameValidationEnabled || string.IsNullOrEmpty(userSettings.UsernameValidationRule))
                return true;

            if (string.IsNullOrEmpty(username))
                return false;

            return userSettings.UsernameValidationUseRegex
                ? Regex.IsMatch(username, userSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : username.All(l => userSettings.UsernameValidationRule.Contains(l));
        }
    }
}
