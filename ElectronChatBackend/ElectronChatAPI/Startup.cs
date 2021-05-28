using ElectronChatCosmosDB;
using ElectronChatCosmosDB.Interfaces;
using ElectronChatCosmosDB.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ElectronChatAPI.Hubs;
using System.Threading.Tasks;
using ElectronChatAPI.Services;

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
            services.AddCors(options =>
                options.AddPolicy("CorsPolicy",
                    builder =>
                        builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials()));

            services.AddMemoryCache();

            services.AddControllers();

            this.InitDatabase();
            this.ConfigureDependencies(services);
            this.ConfigureAuthentication(services);

            services.AddSignalR(options => { options.EnableDetailedErrors = true; });
        }

        public virtual void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Configuration.GetValue<string>("JwtIssuer"),
                        ValidAudience = this.Configuration.GetValue<string>("JwtIssuer"),
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtKey")
                            )),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (string.IsNullOrEmpty(accessToken) == false)
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public virtual void InitDatabase()
        {
            DBConfiguration.Initialize(
                this.Configuration.GetValue<string>("DBEndpointUrl"),
                this.Configuration.GetValue<string>("DBPrimaryKey"),
                this.Configuration.GetValue<string>("DBName"));
        }

        public virtual void ConfigureDependencies(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ElectronChatHub>("/chat");
                endpoints.MapControllers();
            });
        }
    }
}
