using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Feats.Common.Tests;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Feats.Management.Tests
{
    public class StartupTests : TestBase
    {
        [Test]
        public async Task GivenHost_WhenQueryingStatus_ThenWeGetAnswer()
        {
            using var host = this.GivenHost();
            await host.StartAsync();
            try 
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:8003");
                
                var metricsRequest = new HttpRequestMessage(
                    HttpMethod.Get, 
                    new Uri("/health", UriKind.Relative));

                var response = await client.SendAsync(metricsRequest);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }

    public static class StartUpTestsExtensions
    {
        public static IHost GivenHost(this StartupTests tests)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:8003");
            return Program
                .CreateHostBuilder(new string[0])
                .Build();
        }
    }
}