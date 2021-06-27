# ASP.NET Core WebSocket Middleware

WebSockets with Kestrel made easy

``` csharp
// In your configure method:
            var server = new WebSocketConnection();
            server.Start(c =>
            {
                c.OnOpen = socket => Console.WriteLine("OnOpen");
                c.OnClose = socket => Console.WriteLine("OnClose");
                c.OnMessage = async (socket, message) =>
                {
                    Console.WriteLine($"OnMessage: {message}");
                    //await connection.SendAsync(socket, "hello world");
                };
                c.OnBinary = (socket, bytes) =>
                {
                    Encoding.UTF8.GetString(bytes);
                };
            });

            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>(server);

```


