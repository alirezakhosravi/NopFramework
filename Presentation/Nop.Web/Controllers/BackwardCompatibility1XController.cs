using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Users;
using Nop.Services.Seo;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility1XController : BasePublicController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public BackwardCompatibility1XController(IUserService userService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            this._userService = userService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        public virtual IActionResult GeneralRedirect()
        {
            // use Request.RawUrl, for instance to parse out what was invoked
            // this regex will extract anything between a "/" and a ".aspx"
            var regex = new Regex(@"(?<=/).+(?=\.aspx)", RegexOptions.Compiled);
            var rawUrl = _webHelper.GetRawUrl(this.HttpContext.Request);
            var aspxfileName = regex.Match(rawUrl).Value.ToLowerInvariant();

            switch (aspxfileName)
            {
                case "profile":
                    {
                        return RedirectUserProfile(_webHelper.QueryString<string>("UserId"));
                    }
                case "compareproducts":
                    {
                        return RedirectToRoutePermanent("CompareProducts");
                    }
                case "contactus":
                    {
                        return RedirectToRoutePermanent("ContactUs");
                    }
                case "passwordrecovery":
                    {
                        return RedirectToRoutePermanent("PasswordRecovery");
                    }
                case "login":
                    {
                        return RedirectToRoutePermanent("Login");
                    }
                case "register":
                    {
                        return RedirectToRoutePermanent("Register");
                    }
                case "newsarchive":
                    {
                        return RedirectToRoutePermanent("NewsArchive");
                    }
                case "search":
                    {
                        return RedirectToRoutePermanent("ProductSearch");
                    }
                case "sitemap":
                    {
                        return RedirectToRoutePermanent("Sitemap");
                    }
                case "recentlyaddedproducts":
                    {
                        return RedirectToRoutePermanent("NewProducts");
                    }
                case "shoppingcart":
                    {
                        return RedirectToRoutePermanent("ShoppingCart");
                    }
                case "wishlist":
                    {
                        return RedirectToRoutePermanent("Wishlist");
                    }
                case "CheckGiftCardBalance":
                    {
                        return RedirectToRoutePermanent("CheckGiftCardBalance");
                    }
                default:
                    break;
            }

            //no permanent redirect in this case
            return RedirectToRoute("HomePage");
        }

        public virtual IActionResult RedirectUserProfile(string id)
        {
            //we can't use dash in MVC
            var userId = Convert.ToInt32(id);
            var user = _userService.GetUserById(userId);
            if (user == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("UserProfile", new { id = user.Id });
        }

        #endregion
    }
}