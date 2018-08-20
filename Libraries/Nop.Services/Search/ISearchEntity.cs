using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Data;

namespace Nop.Services.Search
{
    public interface ISearchEntity
    {
        IList<SearchResult> GetSearchResults(IDbContext dbContext, string q);
    }
}
