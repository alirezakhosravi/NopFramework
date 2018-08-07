using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Users;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the report model factory implementation
    /// </summary>
    public partial class ReportModelFactory : IReportModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserReportService _userReportService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ReportModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserReportService userReportService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._countryService = countryService;
            this._userReportService = userReportService;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        #region User reports

        /// <summary>
        /// Prepare user reports search model
        /// </summary>
        /// <param name="searchModel">User reports search model</param>
        /// <returns>User reports search model</returns>
        public virtual UserReportsSearchModel PrepareUserReportsSearchModel(UserReportsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            PrepareRegisteredUsersReportSearchModel(searchModel.RegisteredUsers);

            return searchModel;
        }

        /// <summary>
        /// Prepare registered users report search model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>Registered users report search model</returns>
        protected virtual RegisteredUsersReportSearchModel PrepareRegisteredUsersReportSearchModel(RegisteredUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged registered users report list model
        /// </summary>
        /// <param name="searchModel">Registered users report search model</param>
        /// <returns>Registered users report list model</returns>
        public virtual RegisteredUsersReportListModel PrepareRegisteredUsersReportListModel(RegisteredUsersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get report items
            var reportItems = new List<RegisteredUsersReportModel>
            {
                new RegisteredUsersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Users.RegisteredUsers.Fields.Period.7days"),
                    Users = _userReportService.GetRegisteredUsersReport(7)
                },
                new RegisteredUsersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Users.RegisteredUsers.Fields.Period.14days"),
                    Users = _userReportService.GetRegisteredUsersReport(14)
                },
                new RegisteredUsersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Users.RegisteredUsers.Fields.Period.month"),
                    Users = _userReportService.GetRegisteredUsersReport(30)
                },
                new RegisteredUsersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Reports.Users.RegisteredUsers.Fields.Period.year"),
                    Users = _userReportService.GetRegisteredUsersReport(365)
                }
            };

            //prepare list model
            var model = new RegisteredUsersReportListModel
            {
                Data = reportItems.PaginationByRequestModel(searchModel),
                Total = reportItems.Count
            };

            return model;
        }

        #endregion

        #endregion
    }
}