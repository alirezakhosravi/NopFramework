using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Notification;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Notifications
{
    /// <summary>
    /// Queued notification service.
    /// </summary>
    public partial class QueuedNotificationService : IQueuedNotificationService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<QueuedNotification> _queuedNotificationRepository;

        #endregion

        #region Ctor

        public QueuedNotificationService(IDbContext dbContext,
            IEventPublisher eventPublisher,
            IRepository<QueuedNotification> queuedNotificationRepository)
        {
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
            this._queuedNotificationRepository = queuedNotificationRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>        
        public virtual void InsertQueuedNotification(QueuedNotification queuedNotification)
        {
            if (queuedNotification == null)
                throw new ArgumentNullException(nameof(queuedNotification));

            _queuedNotificationRepository.Insert(queuedNotification);

            //event notification
            _eventPublisher.EntityInserted(queuedNotification);
        }

        /// <summary>
        /// Updates a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>
        public virtual void UpdateQueuedNotification(QueuedNotification queuedNotification)
        {
            if (queuedNotification == null)
                throw new ArgumentNullException(nameof(queuedNotification));

            _queuedNotificationRepository.Update(queuedNotification);

            //event notification
            _eventPublisher.EntityUpdated(queuedNotification);
        }

        /// <summary>
        /// Deleted a queued notification
        /// </summary>
        /// <param name="queuedNotification">Queued notification</param>
        public virtual void DeleteQueuedNotification(QueuedNotification queuedNotification)
        {
            if (queuedNotification == null)
                throw new ArgumentNullException(nameof(queuedNotification));

            _queuedNotificationRepository.Delete(queuedNotification);

            //event notification
            _eventPublisher.EntityDeleted(queuedNotification);
        }

        /// <summary>
        /// Deleted a queued notification
        /// </summary>
        /// <param name="queuedNotifications">Queued notification</param>
        public virtual void DeleteQueuedNotifications(IList<QueuedNotification> queuedNotifications)
        {
            if (queuedNotifications == null)
                throw new ArgumentNullException(nameof(queuedNotifications));

            _queuedNotificationRepository.Delete(queuedNotifications);

            //event notification
            foreach (var queuedNotification in queuedNotifications)
            {
                _eventPublisher.EntityDeleted(queuedNotification);
            }
        }

        /// <summary>
        /// Gets a queued notification by identifier
        /// </summary>
        /// <param name="queuedNotificationId">Queued notification identifier</param>
        /// <returns>Queued notification</returns>
        public virtual QueuedNotification GetQueuedNotificationById(int queuedNotificationId)
        {
            if (queuedNotificationId == 0)
                return null;

            return _queuedNotificationRepository.GetById(queuedNotificationId);
        }

        /// <summary>
        /// Get queued notification by identifiers
        /// </summary>
        /// <param name="queuedNotificationIds">queued notification identifiers</param>
        /// <returns>Queued notifications</returns>
        public virtual IList<QueuedNotification> GetQueuedNotificationsByIds(int[] queuedNotificationIds)
        {
            if (queuedNotificationIds == null || queuedNotificationIds.Length == 0)
                return new List<QueuedNotification>();

            var query = from qe in _queuedNotificationRepository.Table
                        where queuedNotificationIds.Contains(qe.Id)
                        select qe;
            var queuedEmails = query.ToList();
            //sort by passed identifiers
            var sortedQueuedEmails = new List<QueuedNotification>();
            foreach (var id in queuedNotificationIds)
            {
                var queuedEmail = queuedEmails.Find(x => x.Id == id);
                if (queuedEmail != null)
                    sortedQueuedEmails.Add(queuedEmail);
            }

            return sortedQueuedEmails;
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only emails for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item list</returns>
        public virtual IPagedList<QueuedNotification> SearchNotifications(string userIds,
            DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _queuedNotificationRepository.Table;
            if (!string.IsNullOrEmpty(userIds))
            {
                var lstUserIds = userIds.Split(',').Select(e => int.Parse(e)).ToList();
                query = query.Where(qe => qe.ListOfUserIds.ToList().Any(q => lstUserIds.Contains(q)));
            }
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            if (loadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = loadNewest ?
                //load the newest records
                query.OrderByDescending(qe => qe.CreatedOnUtc) :
                //load by priority
                query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);

            var queuedEmails = new PagedList<QueuedNotification>(query, pageIndex, pageSize);
            return queuedEmails;
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        public virtual void DeleteAllNotifications()
        {
            //do all databases support "Truncate command"?
            var queuedNotificationTableName = _dbContext.GetTableName<QueuedNotification>();
            _dbContext.ExecuteSqlCommand($"TRUNCATE TABLE [{queuedNotificationTableName}]");

            var queuedNotifications = _queuedNotificationRepository.Table.ToList();
            foreach (var qe in queuedNotifications)
                _queuedNotificationRepository.Delete(qe);
        }

        #endregion
    }
}
