using System;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory implementation
    /// </summary>
    public partial class HomeModelFactory : IHomeModelFactory
    {
        #region Constants

        /// <summary>
        /// nopCommerce news URL
        /// </summary>
        /// <remarks>
        /// {0} : nopCommerce version
        /// {1} : whether the store based is on the localhost
        /// {2} : whether advertisements are hidden
        /// {3} : store URL
        /// </remarks>
        private const string NOP_COMMERCE_NEWS_URL = "https://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}";

        #endregion

        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HomeModelFactory(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHttpContextAccessor httpContextAccessor,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            this._adminAreaSettings = adminAreaSettings;
            this._commonModelFactory = commonModelFactory;
            this._httpContextAccessor = httpContextAccessor;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>Dashboard model</returns>
        public virtual DashboardModel PrepareDashboardModel(DashboardModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            
            //prepare nested search models
            _commonModelFactory.PreparePopularSearchTermSearchModel(model.PopularSearchTerms);
            return model;
        }

        #endregion
    }
}