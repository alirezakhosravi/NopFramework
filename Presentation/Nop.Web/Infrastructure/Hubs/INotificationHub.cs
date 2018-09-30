using System.Threading.Tasks;

namespace Nop.Web.Infrastructure.Hubs
{
    public interface INotificationHub : IBaseHub
    {
        Task SendNotification(string message);
        Task SendNotificationToUser(string user, string message);
    }
}
