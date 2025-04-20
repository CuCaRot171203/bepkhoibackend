using Microsoft.AspNetCore.SignalR;

namespace BepKhoiBackend.API.Hubs
{
    public class OrderHub : Hub
    {
        //public async Task SendOrderUpdate(object order)
        //{
        //    await Clients.All.SendAsync("ReceiveOrderUpdate", order);
        //}
        public async Task JoinOrderGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }

        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }

    }
}
