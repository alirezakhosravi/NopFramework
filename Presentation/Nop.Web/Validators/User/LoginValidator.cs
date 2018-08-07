using FluentValidation;
using Nop.Core.Domain.Users;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.User;

namespace Nop.Web.Validators.User
{
    public partial class LoginValidator : BaseNopValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            if (!userSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
    }
}