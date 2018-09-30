using System;
using System.Collections.Generic;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Media;
using Nop.Services.Common;
using Nop.Services.Users;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;
using Nop.Web.Models.Common;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the profile model factory
    /// </summary>
    public partial class ProfileModelFactory : IProfileModelFactory
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ProfileModelFactory(UserSettings userSettings,
            ICountryService countryService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            MediaSettings mediaSettings)
        {
            this._userSettings = userSettings;
            this._countryService = countryService;
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        public virtual ProfileIndexModel PrepareProfileIndexModel(User user, int? page)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var pagingPosts = false;
            var postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = _userService.FormatUserName(user);
            var title = string.Format(_localizationService.GetResource("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                UserProfileId = user.Id,
            };
            return model;
        }

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Profile info model</returns>
        public virtual ProfileInfoModel PrepareProfileInfoModel(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //avatar
            var avatarUrl = "";
            if (_userSettings.AllowUsersToUploadAvatars)
            {
                avatarUrl = _pictureService.GetPictureUrl(
                 _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.AvatarPictureIdAttribute),
                 _mediaSettings.AvatarPictureSize,
                 _userSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            var locationEnabled = false;
            var location = string.Empty;
            if (_userSettings.ShowUsersLocation)
            {
                locationEnabled = true;

                var countryId = _genericAttributeService.GetAttribute<int>(user, NopUserDefaults.CountryIdAttribute);
                var country = _countryService.GetCountryById(countryId);
                if (country != null)
                {
                    location = _localizationService.GetLocalized(country, x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }

            //total forum posts
            var totalPostsEnabled = false;
            var totalPosts = 0;

            //registration date
            var joinDateEnabled = false;
            var joinDate = string.Empty;

            if (_userSettings.ShowUsersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }

            //birth date
            var dateOfBirthEnabled = false;
            var dateOfBirth = string.Empty;
            if (_userSettings.DateOfBirthEnabled)
            {
                var dob = _genericAttributeService.GetAttribute<DateTime?>(user, NopUserDefaults.DateOfBirthAttribute);
                if (dob.HasValue)
                {
                    dateOfBirthEnabled = true;
                    dateOfBirth = dob.Value.ToString("D");
                }
            }

            var model = new ProfileInfoModel
            {
                UserProfileId = user.Id,
                AvatarUrl = avatarUrl,
                LocationEnabled = locationEnabled,
                Location = location,
                TotalPostsEnabled = totalPostsEnabled,
                TotalPosts = totalPosts.ToString(),
                JoinDateEnabled = joinDateEnabled,
                JoinDate = joinDate,
                DateOfBirthEnabled = dateOfBirthEnabled,
                DateOfBirth = dateOfBirth,
            };

            return model;
        }

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>Profile posts model</returns>
        public virtual ProfilePostsModel PrepareProfilePostsModel(User user, int page)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (page > 0)
            {
                page -= 1;
            }

            var pagerModel = new PagerModel
            {
                ShowTotalSummary = false,
                RouteActionName = "UserProfilePaged",
                UseRouteLinks = true,
                RouteValues = new RouteValues { pageNumber = page, id = user.Id }
            };

            var model = new ProfilePostsModel
            {
                PagerModel = pagerModel,
            };

            return model;
        }

        #endregion
    }
}