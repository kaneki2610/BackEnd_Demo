using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Couchbase.Extensions.DependencyInjection;
using BackendSession2.Core.Repositories;
using BackendSession2.Repository;
using Microsoft.AspNetCore.Mvc;

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
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApiDbContext>(options =>
            //    options.UseSqlite(
            //        Configuration.GetConnectionString("DefaultConnection")
            //    ));

            //services.AddControllers();

            //Add Couchbase service to ASP.NET Core
            services.AddCouchbase(Configuration.GetSection("Couchbase"));
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddOptions();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
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

            //app.UseAuthorization();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });

            //Dispose of Couchbase on ASP.NET Core shutdown
          

        }
    }
}
