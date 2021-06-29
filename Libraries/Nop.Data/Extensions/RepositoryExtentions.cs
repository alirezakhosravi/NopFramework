using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using System;
using System.Linq;

namespace Nop.Data.Extensions
{
    public static class RepositoryExtentions
    {
        /// <summary>
        /// Gets a temporal table
        /// </summary>
        public static IQueryable<TEntity> TemporalTable<TEntity>(this IRepository<TEntity> repository, DateTime? date = null) where TEntity : BaseEntity, ITemporal
        {
            date = (date.HasValue) ? date : DateTime.Now;
            var query = $@"SELECT * FROM {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)} FOR SYSTEM_TIME AS OF '{date.Value.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss")}'";
            return EngineContext.Current.Resolve<IDbContext>().QueryFromSql<TEntity>(query);
        }

        /// <summary>
        /// Get entity from temporal table by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TEntity GetTemporalById<TEntity>(this IRepository<TEntity> repository, object id, DateTime? date = null) where TEntity : BaseEntity, ITemporal
        {
            date = (date.HasValue) ? date : DateTime.Now;
            var query = $@"SELECT * FROM {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)} FOR SYSTEM_TIME AS OF '{date.Value.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss")}'";
            return EngineContext.Current.Resolve<IDbContext>().DynamicSqlQuery<TEntity>(query, System.Data.CommandType.Text).FirstOrDefault(e => e.Id == int.Parse(id.ToString()));
        }

        /// <summary>
        /// Gets list of change
        /// </summary>
        public static IQueryable<TEntity> ChangeTraking<TEntity>(this IRepository<TEntity> repository) where TEntity : BaseEntity, IChangeTracking
        {
            var query = $@"SELECT SYS_CHANGE_VERSION, SYS_CHANGE_CREATION_VERSION, SYS_CHANGE_OPERATION, SYS_CHANGE_COLUMNS, SYS_CHANGE_CONTEXT, J.* FROM CHANGETABLE (CHANGES {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)}, null) CT INNER JOIN {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)} J ON J.Id = CT.Id";
            return EngineContext.Current.Resolve<IDbContext>().DynamicSqlQuery<TEntity>(query, System.Data.CommandType.Text).AsQueryable();
        }
    }
}
