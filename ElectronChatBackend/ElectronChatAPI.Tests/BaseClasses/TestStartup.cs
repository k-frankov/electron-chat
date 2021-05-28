using ElectronChatAPI.Hubs;
using ElectronChatAPI.Services;
using ElectronChatAPI.Tests.BaseClasses.Auth;
using ElectronChatAPI.Tests.Fakes;

using ElectronChatCosmosDB.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronChatAPI.Tests.BaseClasses
{
    public class TestStartup : Startup
    {
        public new IConfiguration Configuration { get; }

        public TestStartup(IConfiguration configuration) : base(configuration)
        {
            Configuration = configuration;
        }

        public override void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme";
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(options =>
            {
                options.SetIdentity("testUser");
            });
        }

        public override void ConfigureDependencies(IServiceCollection services)
        {
            services.AddScoped<IElectronChatHub, ElectronChatHubFake>();
            services.AddScoped<IBlobStorageService, BlobStorageServiceFake>();
            services.AddScoped<IMessageRepository, MessageRepositoryFake>();
        }

        public override void InitDatabase()
        {
        }
    }
}
