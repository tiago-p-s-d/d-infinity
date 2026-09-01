using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinCampaignRoom(int campaignId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"campaign_{campaignId}");
    }

    public async Task LeaveCampaignRoom(int campaignId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"campaign_{campaignId}");
    }
    public async Task UpdateMapState(int campaignId, string mapUrl, double zoom)
    {
        var mapState = new
        {
            campaignId,
            mapUrl,
            zoom
        };
        await Clients.Group($"campaign_{campaignId}").SendAsync("ReceiveMapState", mapState);
    }
}