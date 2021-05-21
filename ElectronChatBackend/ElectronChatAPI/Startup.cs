using ElectronChatCosmosDB;
using ElectronChatCosmosDB.Interfaces;
using ElectronChatCosmosDB.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElectronChatAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            this.InitDatabase();

            services.AddScoped<IUserRepository, UserRepository>();
        }

        protected void InitDatabase()
        {
            DBConfiguration.Initialize(
                this.Configuration.GetValue<string>("DBEndpointUrl"),
                this.Configuration.GetValue<string>("DBPrimaryKey"),
                this.Configuration.GetValue<string>("DBName"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
