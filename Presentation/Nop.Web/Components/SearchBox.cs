using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class SearchBoxViewComponent : NopViewComponent
    {
        
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
