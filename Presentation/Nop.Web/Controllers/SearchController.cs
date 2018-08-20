using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public class SearchController : BasePublicController
    {
        #region Fields

        private readonly ISearchModelFactory _searchModelFactory;

        #endregion

        #region Ctor

        public SearchController(ISearchModelFactory searchModelFactory)
        {
            this._searchModelFactory = searchModelFactory;
        }

        #endregion

        #region Methods
        [HttpGet]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public virtual IActionResult SearchResult(string q)
        {
            var model = _searchModelFactory.GetSearchResult(q);
            return View(model);
        }

        [HttpGet]
        [HttpsRequirement(SslRequirement.NoMatter)]
        public virtual IActionResult SearchTermAutoComplete(string term)
        {
            var model = _searchModelFactory.GetSearchResult(term);
            return Json(model);
        }

        #endregion
    }
}
