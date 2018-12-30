using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Users;
using Nop.Services.Events;

namespace Nop.Services.Users
{
    /// <summary>
    /// User attribute service
    /// </summary>
    public partial class UserAttributeService : IUserAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<UserAttribute> _userAttributeRepository;
        private readonly IRepository<UserAttributeValue> _userAttributeValueRepository;

        #endregion

        #region Ctor

        public UserAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<UserAttribute> userAttributeRepository,
            IRepository<UserAttributeValue> userAttributeValueRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._userAttributeRepository = userAttributeRepository;
            this._userAttributeValueRepository = userAttributeValueRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        public virtual void DeleteUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            _userAttributeRepository.Delete(userAttribute);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(userAttribute);
        }

        /// <summary>
        /// Gets all user attributes
        /// </summary>
        /// <returns>User attributes</returns>
        public virtual IList<UserAttribute> GetAllUserAttributes()
        {
            return _cacheManager.Get(NopUserServiceDefaults.UserAttributesAllCacheKey, () =>
            {
                var query = from ca in _userAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                return query.ToList();
            });
        }

        /// <summary>
        /// Gets a user attribute 
        /// </summary>
        /// <param name="userAttributeId">User attribute identifier</param>
        /// <returns>User attribute</returns>
        public virtual UserAttribute GetUserAttributeById(int userAttributeId)
        {
            if (userAttributeId == 0)
                return null;

            var key = string.Format(NopUserServiceDefaults.UserAttributesByIdCacheKey, userAttributeId);
            return _cacheManager.Get(key, () => _userAttributeRepository.GetById(userAttributeId));
        }

        /// <summary>
        /// Inserts a user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        public virtual void InsertUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            _userAttributeRepository.Insert(userAttribute);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(userAttribute);
        }

        /// <summary>
        /// Updates the user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        public virtual void UpdateUserAttribute(UserAttribute userAttribute)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            _userAttributeRepository.Update(userAttribute);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(userAttribute);
        }

        /// <summary>
        /// Deletes a user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        public virtual void DeleteUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Delete(userAttributeValue);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(userAttributeValue);
        }

        /// <summary>
        /// Gets user attribute values by user attribute identifier
        /// </summary>
        /// <param name="userAttributeId">The user attribute identifier</param>
        /// <returns>User attribute values</returns>
        public virtual IList<UserAttributeValue> GetUserAttributeValues(int userAttributeId)
        {
            var key = string.Format(NopUserServiceDefaults.UserAttributeValuesAllCacheKey, userAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _userAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.UserAttributeId == userAttributeId
                            select cav;
                var userAttributeValues = query.ToList();
                return userAttributeValues;
            });
        }

        /// <summary>
        /// Gets a user attribute value
        /// </summary>
        /// <param name="userAttributeValueId">User attribute value identifier</param>
        /// <returns>User attribute value</returns>
        public virtual UserAttributeValue GetUserAttributeValueById(int userAttributeValueId)
        {
            if (userAttributeValueId == 0)
                return null;

            var key = string.Format(NopUserServiceDefaults.UserAttributeValuesByIdCacheKey, userAttributeValueId);
            return _cacheManager.Get(key, () => _userAttributeValueRepository.GetById(userAttributeValueId));
        }

        /// <summary>
        /// Inserts a user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        public virtual void InsertUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Insert(userAttributeValue);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(userAttributeValue);
        }

        /// <summary>
        /// Updates the user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        public virtual void UpdateUserAttributeValue(UserAttributeValue userAttributeValue)
        {
            if (userAttributeValue == null)
                throw new ArgumentNullException(nameof(userAttributeValue));

            _userAttributeValueRepository.Update(userAttributeValue);

            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopUserServiceDefaults.UserAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(userAttributeValue);
        }

        #endregion
    }
}