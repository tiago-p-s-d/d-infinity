using System.Security.Claims;
using Api.DTOs.Gameplay;
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

    public async Task UpdateMapState(int campaignId, int mapId, string mapUrl, double zoom, int gridCellSize)
    {
        var mapState = new
        {
            campaignId,
            mapId,
            mapUrl,
            zoom,
            gridCellSize
        };

        await Clients.Group($"campaign_{campaignId}").SendAsync("ReceiveMapState", mapState);
    }

    public async Task MoveToken(int campaignId, MoveTokenDto dto)
    {
        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdStr, out var userId);

        var movementPayload = new
        {
            tokenId = dto.TokenId,
            coordX = dto.CoordX,
            coordY = dto.CoordY,
            movedByUserId = userId
        };

        await Clients.Group($"campaign_{campaignId}").SendAsync("ReceiveTokenMoved", movementPayload);
    }

    public async Task NotifySessionEnded(int campaignId)
    {
        await Clients.Group($"campaign_{campaignId}").SendAsync("ReceiveSessionEnded");
    }
}