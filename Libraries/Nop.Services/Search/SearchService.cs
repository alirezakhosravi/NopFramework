using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Services.Search
{
    /// <summary>
    /// search service
    /// </summary>
    public partial class SearchService : ISearchService
    {
        #region Fields

        private readonly ISearchEntity _searchEntity;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public SearchService(ISearchEntity searchEntity,
                             IDbContext dbContext)
        {
            this._searchEntity = searchEntity;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets the seatch result.
        /// </summary>
        /// <returns>The seatch result.</returns>
        /// <param name="q">The search value.</param>
        public IList<SearchResult> GetSeatchResult(string q)
        {
            var model = _searchEntity.GetSearchResults(_dbContext, q);

            return model;
        }
        #endregion
    }
}