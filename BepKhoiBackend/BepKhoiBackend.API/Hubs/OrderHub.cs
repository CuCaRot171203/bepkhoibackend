using Microsoft.AspNetCore.SignalR;

namespace BepKhoiBackend.API.Hubs
{
    public class OrderHub : Hub
    {
        public async Task SendOrderUpdate(object order)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", order);
        }
    }
}
