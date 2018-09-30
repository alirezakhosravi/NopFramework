using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Nop.Web.Infrastructure.Hubs
{
	public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
