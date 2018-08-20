using System.Collections.Generic;
using Nop.Core.Domain.Users;
using Nop.Core.Domain.Security;

namespace Nop.Services.Search
{
    /// <summary>
    /// search service interface
    /// </summary>
    public partial interface ISearchService
    {
        /// <summary>
        /// Gets the seatch result.
        /// </summary>
        /// <returns>The seatch result.</returns>
        /// <param name="g">The search value.</param>
        IList<SearchResult> GetSeatchResult(string g);
    }
}