using System;
using System.Text;
using System.Threading.Tasks;
using chermio.WebSocketServerMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SampleApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var server = new WebSocketConnection();
            server.Start(c =>
            {
                c.OnOpen = socket => Console.WriteLine("OnOpen");
                c.OnClose = socket => Console.WriteLine("OnClose");
                c.OnMessage = async (socket, message) =>
                {
                    Console.WriteLine($"OnMessage: {message}");
                    await c.SendAsync(socket, $"Echo'd Message: {message}");
                };
                c.OnBinary = (socket, bytes) =>
                {
                    Encoding.UTF8.GetString(bytes);
                };
            });
            app.UseWebSockets();
            app.UseWebSocketMiddleware(server);
        }
    }
}
