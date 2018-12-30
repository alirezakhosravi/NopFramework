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
            return repository.Table.FromSql($"SELECT * FROM {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)} FOR SYSTEM_TIME AS OF {{0}}", date.Value.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss"));
        }

        /// <summary>
        /// Get entity from temporal table by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TEntity GetTemporalById<TEntity>(this IRepository<TEntity> repository, object id, DateTime? date = null) where TEntity : BaseEntity, ITemporal
        {
            date = (date.HasValue) ? date : DateTime.Now;
            return repository.Table.FromSql($"SELECT * FROM {EngineContext.Current.Resolve<IDbContext>().GetTableNameByType(typeof(TEntity), true)} FOR SYSTEM_TIME AS OF {{0}}", date.Value.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss")).First(e => e.Id == int.Parse(id.ToString()));
        }
    }
}
