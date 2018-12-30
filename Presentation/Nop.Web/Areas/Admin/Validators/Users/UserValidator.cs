using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nop.Web.Areas.Admin.Models.Users;
using Nop.Core.Domain.Users;
using Nop.Data;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Users
{
    public partial class UserValidator : BaseNopValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IUserService userService,
            UserSettings userSettings,
            IDbContext dbContext)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for user to be in 'Registered' role")
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                //only for registered users
                .When(x => IsRegisteredUserRoleChecked(x, userService));

            //form fields
            if (userSettings.CountryEnabled && userSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.CountryEnabled &&
                userSettings.StateProvinceEnabled &&
                userSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(x.CountryId).Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                            return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("Account.Fields.StateProvince.Required"));
            }
            if (userSettings.CompanyRequired && userSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Company.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress2.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.ZipPostalCode.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.CityRequired && userSettings.CityEnabled)
            {
                RuleFor(x => x.City)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.City.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.CountyRequired && userSettings.CountyEnabled)
            {
                RuleFor(x => x.County)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.County.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Phone.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.FaxRequired && userSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Fax.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }

            SetDatabaseValidationRules<User>(dbContext);
        }

        private bool IsRegisteredUserRoleChecked(UserModel model, IUserService userService)
        {
            var allUserRoles = userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);

            var isInRegisteredRole = newUserRoles.FirstOrDefault(cr => cr.SystemName == NopUserDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
    }
}