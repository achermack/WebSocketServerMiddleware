using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace websockets_middleware
{
    public class WebSocketMiddleware
    {
        private readonly int BUFFER_SIZE = 1024;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly WebSocketConnection _connection;

        public WebSocketMiddleware(RequestDelegate next,  ILoggerFactory loggerFactory, WebSocketConnection connection)
        {
            _logger = loggerFactory.CreateLogger<WebSocketMiddleware>();
            _next = next;
            _connection = connection;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();

                _connection.OnOpen(socket);
                _logger.LogInformation($"WebSocket client connected!");
                await Receive(socket, (result, buffer) =>
                {
                    HandleMessages(result, buffer, socket);
                });
            }
            await _next(context);
        }

        private string ParseUTF8(string UTF8)
        {
            return UTF8.Substring(0, Math.Max(0, UTF8.IndexOf('\0')));
        }

        private void HandleMessages(WebSocketReceiveResult result, byte[] buffer, WebSocket socket)
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    var message = ParseUTF8(Encoding.UTF8.GetString(buffer));
                    _logger.LogInformation($"ClientMessage: {message}");
                    _connection.OnMessage(socket, message);
                    break;
                case WebSocketMessageType.Binary:
                    _connection.OnBinary(socket, buffer);
                    break;
                case WebSocketMessageType.Close:
                    _connection.OnClose(socket);
                    break;
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handler)
        {
            
            while (socket.State == WebSocketState.Open)
            {

                var buffer = new byte[BUFFER_SIZE];
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handler(result, buffer);
            }
        }
    }
}
