using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace ElectronChatAPI.Tests.BaseClasses
{
    public class TestBase
    {
        protected HttpClient client;
        public TestBase()
        {
            IWebHostBuilder builder = new WebHostBuilder().UseStartup<TestStartup>();
            TestServer testServer = new(builder);
            this.client = testServer.CreateClient();
        }
    }
}
