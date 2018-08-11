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
        ChangePassword = 70,
        Avatar = 80,
    }
}