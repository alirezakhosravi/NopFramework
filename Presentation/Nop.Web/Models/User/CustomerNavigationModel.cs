using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.User
{
    public partial class UserNavigationModel : BaseNopModel
    {
        public UserNavigationModel()
        {
            UserNavigationItems = new List<UserNavigationItemModel>();
        }

        public IList<UserNavigationItemModel> UserNavigationItems { get; set; }

        public UserNavigationEnum SelectedTab { get; set; }
    }

    public class UserNavigationItemModel : BaseNopModel
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public UserNavigationEnum Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum UserNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        Orders = 20,
        BackInStockSubscriptions = 30,
        ReturnRequests = 40,
        DownloadableProducts = 50,
        RewardPoints = 60,
        ChangePassword = 70,
        Avatar = 80,
        ForumSubscriptions = 90,
        ProductReviews = 100,
        VendorInfo = 110,
        GdprTools = 120,
        CheckGiftCardBalance = 130
    }
}