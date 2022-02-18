using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Couchbase.Extensions.DependencyInjection;
using BackendSession2.Core.Repositories;
using BackendSession2.Repository;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using BackendSession2.Service;
using BackendSession2.Signalr;

namespace BackendSession2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            //Add Couchbase service to ASP.NET Core
            services.AddCouchbase(Configuration.GetSection("Couchbase"));
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddOptions();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddSingleton<ICacheService, MemoryClassService>();

            services.AddSingleton<IConnectionMultiplexer>(x =>
                ConnectionMultiplexer.Connect(Configuration.GetValue<string>("RedisConnection"))
            );

            //services.AddSignalR() .AddMessagePackProtocol();
            services.AddSignalR(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(5); }).AddMessagePackProtocol();

            //var multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");


            //var db = multiplexer.GetDatabase();
            //if(db.Ping().TotalSeconds > 5)
            //{
            //    Console.WriteLine("====Khong hoat dongggggggggggggggggggggggggggggggg");
            //} else
            //{
            //    var pong = await db.PingAsync();
            //    Console.WriteLine(pong);
            //    Console.WriteLine("hoat dongggggggggggggggggggggggggggggggg");
            //}
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalrHub>("/signalr");
            });

            //app.UseAuthorization();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });

            //Dispose of Couchbase on ASP.NET Core shutdown


        }
    }
}
