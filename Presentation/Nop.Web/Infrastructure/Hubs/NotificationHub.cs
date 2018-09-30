using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.AspNet.SignalR;
using System.Linq;

namespace Nop.Web.Infrastructure.Hubs
{
    public class NotificationHub : Hub , INotificationHub
    {
        protected IHubContext<NotificationHub> _context;

        public NotificationHub(IHubContext<NotificationHub> context)
        {
            _context = context;
        }

        public async Task SendNotification(string message)
        {
            await _context.Clients.All.SendAsync("ReceiveUserNotification", message);
        }

        public async Task SendNotificationToUser(string user, string message)
        {
            await _context.Clients.User(user).SendAsync("ReceiveUserNotification", message);
        }
    }
}
