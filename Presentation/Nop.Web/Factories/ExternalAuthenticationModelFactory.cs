using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Web.Models.User;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the external authentication model factory
    /// </summary>
    public partial class ExternalAuthenticationModelFactory : IExternalAuthenticationModelFactory
    {
        #region Fields

        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExternalAuthenticationModelFactory(IExternalAuthenticationService externalAuthenticationService,
            IWorkContext workContext)
        {
            this._externalAuthenticationService = externalAuthenticationService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>List of the external authentication method model</returns>
        public virtual List<ExternalAuthenticationMethodModel> PrepareExternalMethodsModel()
        {
            return _externalAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_workContext.CurrentUser)
                .Select(authenticationMethod => new ExternalAuthenticationMethodModel
                {
                    ViewComponentName = authenticationMethod.GetPublicViewComponentName()
                }).ToList();
        }

        #endregion
    }
}
