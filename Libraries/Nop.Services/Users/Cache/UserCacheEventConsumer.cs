using Nop.Core.Caching;
using Nop.Core.Domain.Users;
using Nop.Services.Events;

namespace Nop.Services.Users.Cache
{
    /// <summary>
    /// User cache event consumer (used for caching of current user password)
    /// </summary>
    public partial class UserCacheEventConsumer : IConsumer<UserPasswordChangedEvent>
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public UserCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        //password changed
        public void HandleEvent(UserPasswordChangedEvent eventMessage)
        {
            _cacheManager.Remove(string.Format(NopUserServiceDefaults.UserPasswordLifetimeCacheKey, eventMessage.Password.UserId));
        }

        #endregion
    }
}