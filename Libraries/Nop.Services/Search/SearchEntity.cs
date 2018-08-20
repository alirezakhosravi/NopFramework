using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Services.Search
{
    public class SearchEntity : ISearchEntity
    {
        #region Fields

        private readonly ITypeFinder _typeFinder;

        #endregion

        #region ctor
        public SearchEntity(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }
        #endregion

        #region Method

        public IList<Type> GetSearchablesEntity()
        {
            var types = _typeFinder.FindClassesOfType<ISearchable>(_typeFinder.GetAssemblies().Where(e=>e.GetName().ToString().ToLower().Contains("nop.core"))).ToList();

            return types;
        }

        #endregion
    }
}
