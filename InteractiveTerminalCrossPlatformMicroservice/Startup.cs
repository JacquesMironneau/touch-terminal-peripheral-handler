using System;
using Microsoft.OpenApi.Models;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using InteractiveTerminalCrossPlatformMicroservice.PeripheralCreation;
using InteractiveTerminalCrossPlatformMicroservice.PeripheralRequestHandler;
using System.Threading.Tasks;
using InteractiveTerminalCrossPlatformMicroservice.SwaggerCustom;

namespace InteractiveTerminalCrossPlatformMicroservice
{
    /// <summary>
    /// ASP.CORE Startup object, handle the initialisation and set up of the application 
    /// </summary>

    public class Startup
    {

        // Url chosen for the websocket request entrypoint

        private const string WEBSOCKET_URL = "/ws";

        public IConfiguration Configuration { get; }

        private IServiceCollection myservices;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.myservices = services;

            // Handle every origin for the Cors Policy
            services.AddCors(options => options.AddPolicy("APolicy?", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddControllers();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Allow Swagger usage
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.DocumentFilter<CustomInstrospectionFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Websocket initialisation and handling
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(10),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=CommunicateToPeripheral}/{id?}");
                    
            });


            app.Use(async (context, next) =>
            {
                // Wait for request on the Specified WS url
                if (context.Request.Path == WEBSOCKET_URL)
                {
                    // Check if the request is a websocket one (and not HTTP for instance)
                    if (context.WebSockets.IsWebSocketRequest)
                    {

                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        TaskCompletionSource<object> socketFinishedTcs = new TaskCompletionSource<object>();

                        SocketHandler socketHandler = new SocketHandler(webSocket, socketFinishedTcs);
                        PeripheralFactory.SetHandler(new PeripheralEventHandler(socketHandler));

                        await socketFinishedTcs.Task;
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Test Microservice IPM France");
            });
            PeripheralFactory.Init();
        }
    }
}
