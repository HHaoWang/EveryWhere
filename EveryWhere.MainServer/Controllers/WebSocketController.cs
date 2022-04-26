using System.Net.WebSockets;
using System.Text;
using EveryWhere.MainServer.Infrastructure.Websocket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EveryWhere.MainServer.Controllers;

[Route("ws")]
[ApiController]
public class WebSocketController : ControllerBase
{
    private readonly Hub _hub;
    private readonly ILogger<WebSocketController> _logger;

    public WebSocketController(Hub hub, ILogger<WebSocketController> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    [HttpGet("desktop/{computerId}")]
    public async Task Connect(string computerId)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("已连接")), WebSocketMessageType.Text, true, CancellationToken.None);

        await _hub.AddSocket(webSocket,computerId);
    }
}