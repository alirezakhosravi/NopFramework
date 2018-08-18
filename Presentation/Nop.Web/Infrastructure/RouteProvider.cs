using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //reorder routes so the most used ones are on top. It can improve performance

            //areas
            routeBuilder.MapRoute(name: "areaRoute", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //home page
            routeBuilder.MapLocalizedRoute("HomePage", "",
				new { controller = "Home", action = "Index" });

            //login
            routeBuilder.MapLocalizedRoute("Login", "login/",
				new { controller = "User", action = "Login" });

            //register
            routeBuilder.MapLocalizedRoute("Register", "register/",
				new { controller = "User", action = "Register" });

            //logout
            routeBuilder.MapLocalizedRoute("Logout", "logout/",
				new { controller = "User", action = "Logout" });
            
            //user account links
            routeBuilder.MapLocalizedRoute("UserInfo", "user/info",
				new { controller = "User", action = "Info" });

            routeBuilder.MapLocalizedRoute("UserAddresses", "user/addresses",
				new { controller = "User", action = "Addresses" });
            
            routeBuilder.MapLocalizedRoute("UserAddressEdit", "user/addressedit/{addressId:min(0)}",
                new { controller = "User", action = "AddressEdit" });

            routeBuilder.MapLocalizedRoute("UserAddressAdd", "user/addressadd",
                new { controller = "User", action = "AddressAdd" });


            routeBuilder.MapLocalizedRoute("UserChangePassword", "user/changepassword",
                new { controller = "User", action = "ChangePassword" });
            
            //contact us
            routeBuilder.MapLocalizedRoute("ContactUs", "contactus",
				new { controller = "Common", action = "ContactUs" });

            //sitemap
            routeBuilder.MapLocalizedRoute("Sitemap", "sitemap",
				new { controller = "Common", action = "Sitemap" });
            
            //change language (AJAX link)
            routeBuilder.MapLocalizedRoute("ChangeLanguage", "changelanguage/{langid:min(0)}",
				new { controller = "Common", action = "SetLanguage" });
            
            //downloads
            routeBuilder.MapRoute("GetSampleDownload", "download/sample/{productid:min(0)}",
				new { controller = "Download", action = "Sample" });
            
            //login page for checkout as guest
            routeBuilder.MapLocalizedRoute("LoginCheckoutAsGuest", "login/checkoutasguest",
				new { controller = "User", action = "Login", checkoutAsGuest = true });

            //register result page
            routeBuilder.MapLocalizedRoute("RegisterResult", "registerresult/{resultId:min(0)}",
				new { controller = "User", action = "RegisterResult" });

            //check username availability
            routeBuilder.MapLocalizedRoute("CheckUsernameAvailability", "user/checkusernameavailability",
				new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            routeBuilder.MapLocalizedRoute("PasswordRecovery", "passwordrecovery",
				new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            routeBuilder.MapLocalizedRoute("PasswordRecoveryConfirm", "passwordrecovery/confirm",
				new { controller = "User", action = "PasswordRecoveryConfirm" });
            
            //user profile page
            routeBuilder.MapLocalizedRoute("UserProfile", "profile/{id:min(0)}",
				new { controller = "Profile", action = "Index" });

            routeBuilder.MapLocalizedRoute("UserProfilePaged", "profile/{id}/page/{pageNumber:min(0)}",
				new { controller = "Profile", action = "Index" });
            
            //order downloads
            routeBuilder.MapRoute("GetDownload", "download/getdownload/{orderItemId:guid}/{agree?}",
				new { controller = "Download", action = "GetDownload" });

            routeBuilder.MapRoute("GetLicense", "download/getlicense/{orderItemId:guid}/",
				new { controller = "Download", action = "GetLicense" });

            routeBuilder.MapLocalizedRoute("DownloadUserAgreement", "user/useragreement/{orderItemId:guid}",
				new { controller = "User", action = "UserAgreement" });

            routeBuilder.MapRoute("GetOrderNoteFile", "download/ordernotefile/{ordernoteid:min(0)}",
				new { controller = "Download", action = "GetOrderNoteFile" });
            
            //get state list by country ID  (AJAX link)
            routeBuilder.MapRoute("GetStatesByCountryId", "country/getstatesbycountryid/",
				new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            routeBuilder.MapRoute("EuCookieLawAccept", "eucookielawaccept",
				new { controller = "Common", action = "EuCookieLawAccept" });
            
            //product attributes with "upload file" type
            routeBuilder.MapLocalizedRoute("UploadFileProductAttribute", "uploadfileproductattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

            //checkout attributes with "upload file" type
            routeBuilder.MapLocalizedRoute("UploadFileCheckoutAttribute", "uploadfilecheckoutattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support
            routeBuilder.MapLocalizedRoute("UploadFileReturnRequest", "uploadfilereturnrequest",
				new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });
            
    //        //private messages
    //        routeBuilder.MapLocalizedRoute("PrivateMessages", "privatemessages/{tab?}",
				//new { controller = "PrivateMessages", action = "Index" });

    //        routeBuilder.MapLocalizedRoute("PrivateMessagesPaged", "privatemessages/{tab?}/page/{pageNumber:min(0)}",
				//new { controller = "PrivateMessages", action = "Index" });

    //        routeBuilder.MapLocalizedRoute("PrivateMessagesInbox", "inboxupdate",
				//new { controller = "PrivateMessages", action = "InboxUpdate" });

    //        routeBuilder.MapLocalizedRoute("PrivateMessagesSent", "sentupdate",
				//new { controller = "PrivateMessages", action = "SentUpdate" });

    //        routeBuilder.MapLocalizedRoute("SendPM", "sendpm/{toUserId:min(0)}",
				//new { controller = "PrivateMessages", action = "SendPM" });

    //        routeBuilder.MapLocalizedRoute("SendPMReply", "sendpm/{toUserId:min(0)}/{replyToMessageId:min(0)}",
				//new { controller = "PrivateMessages", action = "SendPM" });

    //        routeBuilder.MapLocalizedRoute("ViewPM", "viewpm/{privateMessageId:min(0)}",
				//new { controller = "PrivateMessages", action = "ViewPM" });

    //        routeBuilder.MapLocalizedRoute("DeletePM", "deletepm/{privateMessageId:min(0)}",
				//new { controller = "PrivateMessages", action = "DeletePM" });
            
            //robots.txt
            routeBuilder.MapRoute("robots.txt", "robots.txt",
				new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            routeBuilder.MapLocalizedRoute("sitemap.xml", "sitemap.xml",
				new { controller = "Common", action = "SitemapXml" });

            routeBuilder.MapLocalizedRoute("sitemap-indexed.xml", "sitemap-{Id:min(0)}.xml",
				new { controller = "Common", action = "SitemapXml" });
            
            //install
            routeBuilder.MapRoute("Installation", "install",
				new { controller = "Install", action = "Index" });

            //error page
            routeBuilder.MapLocalizedRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            routeBuilder.MapLocalizedRoute("PageNotFound", "page-not-found", 
                new { controller = "Common", action = "PageNotFound" });

            //search
            routeBuilder.MapLocalizedRoute("search", "search/{q}",
                new { controller = "Search", action = "Search" });

            routeBuilder.MapLocalizedRoute("SearchAutoComplete", "searchautocomplete/{q}",
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
