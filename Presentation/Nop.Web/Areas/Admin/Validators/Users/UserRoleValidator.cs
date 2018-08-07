using FluentValidation;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Core.Domain.Users;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Users
{
    public partial class UserRoleValidator : BaseNopValidator<UserRoleModel>
    {
        public UserRoleValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));

            SetDatabaseValidationRules<UserRole>(dbContext);
        }
    }
}