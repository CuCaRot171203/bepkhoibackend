using Microsoft.AspNetCore.SignalR;

namespace BepKhoiBackend.API.Hubs
{
    public class OrderHub : Hub
    {
        public async Task SendOrderUpdate(object order)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", order);
        }

        public async Task SendOrderUpdate(int orderId)
        {
            // Gửi lại cho tất cả client sự kiện 'ReceiveOrderUpdate'
            await Clients.All.SendAsync("ReceiveOrderUpdate", orderId);
        }
    }
}
