using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace EveryWhere.MainServer.Infrastructure.Websocket;

public class Hub
{
    private readonly ILogger<Hub> _logger;

    private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

    public Hub(ILogger<Hub> logger)
    {
        _logger = logger;
    }

    private async Task RemoveClosedConnections()
    {
        foreach ((string? key, WebSocket? socket) in _connections)
        {
            if (socket.State == WebSocketState.Open) continue;
            if (!_connections.Remove(key, out WebSocket? closedSocket)) continue;
            if (closedSocket.State == WebSocketState.Open)
            {
                await closedSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connection closed", CancellationToken.None);
            }
        }
    }

    /// <summary>
    /// 获取所有 sockets 的字典集合
    /// </summary>
    /// <returns></returns>
    public async Task<ConcurrentDictionary<string, WebSocket>> GetAllConnections()
    {
        await RemoveClosedConnections();
        return _connections;
    }

    /// <summary>
    /// 获取指定 id 的 socket
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<WebSocket?> GetSocketById(string id)
    {
        await RemoveClosedConnections();
        _connections.TryGetValue(id,out WebSocket? socket);
        if (socket == null)
        {
            _logger.LogInformation($"找不到ID为{id}的连接");
        }
        return socket;
    }

    /// <summary>
    /// 根据 socket 获取其 id
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public async Task<string> GetId(WebSocket socket)
    {
        await RemoveClosedConnections();
        return _connections.FirstOrDefault(x => x.Value == socket).Key;
    }

    /// <summary>
    /// 删除指定 id 的 socket，并关闭该链接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task RemoveSocketAsync(string id)
    {
        await RemoveClosedConnections();
        if (_connections.TryRemove(id, out WebSocket? socket))
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connection closed",
                CancellationToken.None);
        }
    }

    /// <summary>
    /// 添加一个 socket
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="id"></param>
    public async Task AddSocket(WebSocket socket,string id)
    {
        await RemoveClosedConnections();
        _connections.TryAdd(id, socket);
        byte[] buffer = new byte[1024 * 4];
        WebSocketReceiveResult res = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!res.CloseStatus.HasValue)
        {
            string cmd = Encoding.UTF8.GetString(buffer, 0, res.Count);
            if (!string.IsNullOrEmpty(cmd))
            {
                _logger.LogInformation(cmd);
            }
            res = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        await socket.CloseAsync(res.CloseStatus.Value, res.CloseStatusDescription, CancellationToken.None);
        await RemoveSocketAsync(id);
    }

    public async Task<bool> PushMessage(string id, string message)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        WebSocket? socket = await GetSocketById(id);
        if (socket == null)
        {
            return false;
        }

        await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true,
            CancellationToken.None);
        return true;
    }

    public async Task<bool> NotifyPrinter(string computerId, int jobId)
    {
        return await PushMessage(computerId, jobId.ToString());
    }
}