using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using ElectronChatAPI.Tests.BaseClasses;

using Xunit;

namespace ElectronChatAPI.Tests
{
    public class ShareController_Should_ : TestBase
    {
        [Fact]
        public async Task Return_Bad_Request_If_File_Bigger_Than_5_Megabyte()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uriBuilder = new(codeBase);
            string path = Uri.UnescapeDataString(uriBuilder.Path);
            string assemblyDirectory = Path.GetDirectoryName(path);

            string fileName = Path.Combine(assemblyDirectory, "TestData/test.bin");
            using MemoryStream stream = new(File.ReadAllBytes(fileName));

            MultipartFormDataContent form = new();
            byte[] content = stream.ToArray();
            ByteArrayContent byteArrayContent = new(content, 0, content.Length);
            byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            form.Add(byteArrayContent, "test.bin", "test.bin");

            HttpResponseMessage responseMessage = await base.client.PostAsync("/api/share/first", form);
            Assert.False(responseMessage.IsSuccessStatusCode);

            string response = await responseMessage.Content.ReadAsStringAsync();
            Assert.Equal("File is too big", response);
        }
    }
}
