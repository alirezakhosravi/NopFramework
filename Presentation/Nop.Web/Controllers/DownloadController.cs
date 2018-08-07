using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Users;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Web.Controllers
{
    public partial class DownloadController : BasePublicController
    {
        private readonly UserSettings _userSettings;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public DownloadController(UserSettings userSettings,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._userSettings = userSettings;
            this._downloadService = downloadService;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        public virtual IActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

    }
}