using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Services.Search;
using Nop.Web.Models.Search;

namespace Nop.Web.Factories
{
    public class SearchModelFactory : ISearchModelFactory
    {
        #region Fields

        private readonly ISearchService _searchService;

        #endregion


        #region ctor
        public SearchModelFactory(ISearchService searchService)
        {
            this._searchService = searchService;
        }
        #endregion



        #region Method

        /// <summary>
        /// Gets the search result.
        /// </summary>
        /// <returns>The search result.</returns>
        /// <param name="q">value of search</param>
        public IList<SearchModel> GetSearchResult(string q)
        {
            var result = _searchService.GetSeatchResult(q);

            var model = result.Select(e => new SearchModel
            {
                Title = e.Name,
                Description = e.Description,
                Route = e.Route
            }).ToList();

            return model;
        }

        #endregion
    }
}
