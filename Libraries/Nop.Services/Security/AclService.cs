using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Security
{
    /// <summary>
    /// ACL service
    /// </summary>
    public partial class AclService : IAclService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AclService(IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRecordRepository,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            this._eventPublisher = eventPublisher;
            this._aclRecordRepository = aclRecordRepository;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Delete(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(NopSecurityDefaults.AclRecordPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(aclRecord);
        }

        /// <summary>
        /// Gets an ACL record
        /// </summary>
        /// <param name="aclRecordId">ACL record identifier</param>
        /// <returns>ACL record</returns>
        public virtual AclRecord GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            return _aclRecordRepository.GetById(aclRecordId);
        }

        /// <summary>
        /// Gets ACL records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>ACL records</returns>
        public virtual IList<AclRecord> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from ur in _aclRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName
                        select ur;
            var aclRecords = query.ToList();
            return aclRecords;
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Insert(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(NopSecurityDefaults.AclRecordPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(aclRecord);
        }

        /// <summary>
        /// Inserts an ACL record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="userRoleId">User role id</param>
        /// <param name="entity">Entity</param>
        public virtual void InsertAclRecord<T>(T entity, int userRoleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (userRoleId == 0)
                throw new ArgumentOutOfRangeException(nameof(userRoleId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                UserRoleId = userRoleId
            };

            InsertAclRecord(aclRecord);
        }

        /// <summary>
        /// Updates the ACL record
        /// </summary>
        /// <param name="aclRecord">ACL record</param>
        public virtual void UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Update(aclRecord);

            //cache
            _cacheManager.RemoveByPattern(NopSecurityDefaults.AclRecordPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(aclRecord);
        }

        /// <summary>
        /// Find user role identifiers with granted access
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>User role identifiers</returns>
        public virtual int[] GetUserRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(NopSecurityDefaults.AclRecordByEntityIdNameCacheKey, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from ur in _aclRecordRepository.Table
                            where ur.EntityId == entityId &&
                            ur.EntityName == entityName
                            select ur.UserRoleId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return Authorize(entity, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize ACL permission
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, User user) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (user == null)
                return false;
            
            if (!entity.SubjectToAcl)
                return true;

            foreach (var role1 in user.UserRoles.Where(cr => cr.Active))
                foreach (var role2Id in GetUserRoleIdsWithAccess(entity))
                    if (role1.Id == role2Id)
                        //yes, we have such permission
                        return true;

            //no permission found
            return false;
        }

        #endregion
    }
}