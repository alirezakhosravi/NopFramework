using System;
using System.Collections.Generic;
using Nop.Core;

namespace Nop.Services.Search
{
    public interface ISearchEntity
    {
        IList<Type> GetSearchablesEntity();
    }
}
