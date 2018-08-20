using System;
using System.Collections.Generic;
using Nop.Web.Models.Search;

namespace Nop.Web.Factories
{
    public interface ISearchModelFactory
    {
        /// <summary>
        /// Gets the search result.
        /// </summary>
        /// <returns>The search result.</returns>
        /// <param name="q">value of search</param>
        IList<SearchModel> GetSearchResult(string q);
    }
}
