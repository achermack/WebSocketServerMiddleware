# ASP.NET Core WebSocket Middleware

This middleware uses the System.Net.WebSockets library to easily configure Kestrel to accept incoming WebSocket connections and define handlers for them, similiarly to Fleck


# Usage

``` csharp
// First, you want to configure the port of your websocket server
// In Program.cs

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:8181");
                });

// In Startup.cs configure method, you want to configure how each connection is handled by the middleware
// Inspired by Fleck

            var _newConnection = new WebSocketConnection();
            _newConnection.Start(c =>
            {
                _newConnection.OnOpen = socket => Console.WriteLine("OnOpen");
                _newConnection.OnClose = socket => Console.WriteLine("OnClose");
                _newConnection.OnMessage = async (socket, message) =>
                {
                    Console.WriteLine($"OnMessage: {message}");
                    await _newConnection.SendAsync(socket, $"echo: {message}");
                };
                _newConnection.OnBinary = (socket, bytes) =>
                {
                    Encoding.UTF8.GetString(bytes);
                };
            });

            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>(server);

```


