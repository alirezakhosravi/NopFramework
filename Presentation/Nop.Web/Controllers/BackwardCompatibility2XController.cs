using Microsoft.AspNetCore.Mvc;
using Nop.Services.Seo;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BasePublicController
    {
        #region Fields

        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public BackwardCompatibility2XController(IUrlRecordService urlRecordService)
        {
            this._urlRecordService = urlRecordService;

        }

        #endregion

        #region Methods

        #endregion
    }
}