using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Installation;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            //reorder routes so the most used ones are on top. It can improve performance

            //areas
            routeBuilder.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //home page
            routeBuilder.MapControllerRoute("HomePage", "",
    new { controller = "Home", action = "Index" });

            //login
            routeBuilder.MapControllerRoute("Login", "login/",
    new { controller = "User", action = "Login" });

            //register
            routeBuilder.MapControllerRoute("Register", "register/",
    new { controller = "User", action = "Register" });

            //logout
            routeBuilder.MapControllerRoute("Logout", "logout/",
    new { controller = "User", action = "Logout" });

            //user account links
            routeBuilder.MapControllerRoute("UserInfo", "user/info",
    new { controller = "User", action = "Info" });

            routeBuilder.MapControllerRoute("UserAddresses", "user/addresses",
    new { controller = "User", action = "Addresses" });

            routeBuilder.MapControllerRoute("UserAddressEdit", "user/addressedit/{addressId:min(0)}",
                new { controller = "User", action = "AddressEdit" });

            routeBuilder.MapControllerRoute("UserAddressAdd", "user/addressadd",
                new { controller = "User", action = "AddressAdd" });


            routeBuilder.MapControllerRoute("UserChangePassword", "user/changepassword",
                new { controller = "User", action = "ChangePassword" });

            //contact us
            routeBuilder.MapControllerRoute("ContactUs", "contactus",
    new { controller = "Common", action = "ContactUs" });

            //sitemap
            routeBuilder.MapControllerRoute("Sitemap", "sitemap",
    new { controller = "Common", action = "Sitemap" });

            //change language (AJAX link)
            routeBuilder.MapControllerRoute("ChangeLanguage", "changelanguage/{langid:min(0)}",
    new { controller = "Common", action = "SetLanguage" });

            //downloads
            routeBuilder.MapControllerRoute("GetSampleDownload", "download/sample/{productid:min(0)}",
    new { controller = "Download", action = "Sample" });

            //login page for checkout as guest
            routeBuilder.MapControllerRoute("LoginCheckoutAsGuest", "login/checkoutasguest",
    new { controller = "User", action = "Login", checkoutAsGuest = true });

            //register result page
            routeBuilder.MapControllerRoute("RegisterResult", "registerresult/{resultId:min(0)}",
    new { controller = "User", action = "RegisterResult" });

            //check username availability
            routeBuilder.MapControllerRoute("CheckUsernameAvailability", "user/checkusernameavailability",
    new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            routeBuilder.MapControllerRoute("PasswordRecovery", "passwordrecovery",
    new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            routeBuilder.MapControllerRoute("PasswordRecoveryConfirm", "passwordrecovery/confirm",
    new { controller = "User", action = "PasswordRecoveryConfirm" });

            //user profile page
            routeBuilder.MapControllerRoute("UserProfile", "profile/{id:min(0)}",
    new { controller = "Profile", action = "Index" });

            routeBuilder.MapControllerRoute("UserProfilePaged", "profile/{id}/page/{pageNumber:min(0)}",
    new { controller = "Profile", action = "Index" });

            //order downloads
            routeBuilder.MapControllerRoute("GetDownload", "download/getdownload/{orderItemId:guid}/{agree?}",
    new { controller = "Download", action = "GetDownload" });

            routeBuilder.MapControllerRoute("GetLicense", "download/getlicense/{orderItemId:guid}/",
    new { controller = "Download", action = "GetLicense" });

            routeBuilder.MapControllerRoute("DownloadUserAgreement", "user/useragreement/{orderItemId:guid}",
    new { controller = "User", action = "UserAgreement" });

            routeBuilder.MapControllerRoute("GetOrderNoteFile", "download/ordernotefile/{ordernoteid:min(0)}",
    new { controller = "Download", action = "GetOrderNoteFile" });

            //get state list by country ID  (AJAX link)
            routeBuilder.MapControllerRoute("GetStatesByCountryId", "country/getstatesbycountryid/",
    new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            routeBuilder.MapControllerRoute("EuCookieLawAccept", "eucookielawaccept",
    new { controller = "Common", action = "EuCookieLawAccept" });

            //product attributes with "upload file" type
            routeBuilder.MapControllerRoute("UploadFileProductAttribute", "uploadfileproductattribute/{attributeId:min(0)}",
    new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

            //checkout attributes with "upload file" type
            routeBuilder.MapControllerRoute("UploadFileCheckoutAttribute", "uploadfilecheckoutattribute/{attributeId:min(0)}",
    new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support
            routeBuilder.MapControllerRoute("UploadFileReturnRequest", "uploadfilereturnrequest",
    new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //        //private messages
            //        routeBuilder.MapControllerRoute("PrivateMessages", "privatemessages/{tab?}",
            //new { controller = "PrivateMessages", action = "Index" });

            //        routeBuilder.MapControllerRoute("PrivateMessagesPaged", "privatemessages/{tab?}/page/{pageNumber:min(0)}",
            //new { controller = "PrivateMessages", action = "Index" });

            //        routeBuilder.MapControllerRoute("PrivateMessagesInbox", "inboxupdate",
            //new { controller = "PrivateMessages", action = "InboxUpdate" });

            //        routeBuilder.MapControllerRoute("PrivateMessagesSent", "sentupdate",
            //new { controller = "PrivateMessages", action = "SentUpdate" });

            //        routeBuilder.MapControllerRoute("SendPM", "sendpm/{toUserId:min(0)}",
            //new { controller = "PrivateMessages", action = "SendPM" });

            //        routeBuilder.MapControllerRoute("SendPMReply", "sendpm/{toUserId:min(0)}/{replyToMessageId:min(0)}",
            //new { controller = "PrivateMessages", action = "SendPM" });

            //        routeBuilder.MapControllerRoute("ViewPM", "viewpm/{privateMessageId:min(0)}",
            //new { controller = "PrivateMessages", action = "ViewPM" });

            //        routeBuilder.MapControllerRoute("DeletePM", "deletepm/{privateMessageId:min(0)}",
            //new { controller = "PrivateMessages", action = "DeletePM" });

            //robots.txt
            routeBuilder.MapControllerRoute("robots.txt", "robots.txt",
    new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            routeBuilder.MapControllerRoute("sitemap.xml", "sitemap.xml",
    new { controller = "Common", action = "SitemapXml" });

            routeBuilder.MapControllerRoute("sitemap-indexed.xml", "sitemap-{Id:min(0)}.xml",
    new { controller = "Common", action = "SitemapXml" });

            //install
            routeBuilder.MapControllerRoute("Installation", "install",
    new { controller = "Install", action = "Index" });

            //error page
            routeBuilder.MapControllerRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            routeBuilder.MapControllerRoute("PageNotFound", "page-not-found",
                new { controller = "Common", action = "PageNotFound" });

            //search
            routeBuilder.MapControllerRoute("Search", "search/",
                new { controller = "Search", action = "SearchResult" });

            routeBuilder.MapControllerRoute("SearchAutoComplete", "searchautocomplete/",
                new { controller = "Search", action = "SearchTermAutoComplete" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return 0; }
        }

        #endregion
    }
}
