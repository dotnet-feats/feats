using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class FeatsEvaluationClientTests
    {
        [Test]
        public async Task GivenAClientWithCache_WhenRequestingAnEvaluationReturnsTrue_ThenWeGetTrueFromCache()
        {
            var url = "http://localhost:8001";
            using var host = this.GivenServer(url, featureIsOn: true);
            var configuration = this.GivenConfiguration("roger");
            var cache = this.GivenCache(value: true);
            await host.StartAsync();

            var request = new FeatureEvaluationRequestBuilder()
                .WithName("a")
                .WithPath("b")
                .WithIsGreaterThan(5)
                .Build();
                
            try
            {
                await this
                    .GivenClient(
                        configuration.Object,
                        cache,
                        host.Services.GetService<IHttpClientFactory>())
                    .IsOn(request, CancellationToken.None)
                    .ThenWeGet(true);
            }
            finally
            {
                await host.StopAsync();
            }
        }

        [Test]
        public async Task GivenAClientWithNoCache_WhenRequestingAnEvaluationReturnsTrue_ThenWeGetTrue()
        {
            var url = "http://localhost:8001";
            using var host = this.GivenServer(url, featureIsOn: true);
            var configuration = this.GivenConfiguration(url);
            var cache = this.GivenNoCache();
            await host.StartAsync();
            
            var request = new FeatureEvaluationRequestBuilder()
                .WithName("a")
                .WithPath("b")
                .WithIsGreaterThan(5)
                .Build();
            
            try
            {
                await this
                    .GivenClient(
                        configuration.Object,
                        cache,
                        host.Services.GetService<IHttpClientFactory>())
                    .IsOn(request, CancellationToken.None)
                    .ThenWeGet(true);
            }
            finally
            {
                await host.StopAsync();
            }
        }
        
        [Test]
        public async Task GivenAClientWithNoCache_WhenRequestingAnEvaluationReturnsFalse_ThenWeGetFalse()
        {
            var url = "http://localhost:8001";
            using var host = this.GivenServer(url, featureIsOn: false);
            var configuration = this.GivenConfiguration(url);
            var cache = this.GivenNoCache();
            await host.StartAsync();
            
            var request = new FeatureEvaluationRequestBuilder()
                .WithName("a")
                .WithPath("b")
                .WithIsGreaterThan(5)
                .Build();
            
            try
            {
                await this
                    .GivenClient(
                        configuration.Object,
                        cache,
                        host.Services.GetService<IHttpClientFactory>())
                    .IsOn(request, CancellationToken.None)
                    .ThenWeGet(false);
            }
            finally
            {
                await host.StopAsync();
            }
        }
    }

    internal static class FeatsEvaluationClientTestsExtensions
    {
        internal static IEvaluationCache GivenCache(
            this FeatsEvaluationClientTests tests,
            bool value)
        {
            return new TestCacheEvaluationCache(value);
        }
        
        internal static IEvaluationCache GivenNoCache(
            this FeatsEvaluationClientTests tests)
        {
            return new TestNoCacheEvaluationCache();
        }

        internal static Mock<IFeatsEvaluationConfiguration> GivenConfiguration(
            this FeatsEvaluationClientTests tests,
            string url)
        {
            var builder = new UriBuilder(url);
            var uri = builder.Uri;
            
            var mock = new Mock<IFeatsEvaluationConfiguration>();
            mock
                .Setup(_ => _.Host)
                .Returns(uri);
            mock
                .Setup(_ => _.RequestTimeout)
                .Returns(1.Seconds());
            
            return mock;
        }

        internal static IHost GivenServer(
            this FeatsEvaluationClientTests tests,
            string url,
            bool featureIsOn)
        {
            return Host
                .CreateDefaultBuilder(null)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.AddHttpClient();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGet("/features", async context =>
                                {
                                    context.Request.Headers.Keys.Should().Contain("Accept");
                                    context.Request.Headers.Keys.Should().Contain(StrategySettings.GreaterThan);
                                    await context.Response.WriteAsync(JsonSerializer.Serialize(featureIsOn));
                                });
                            });
                        });
                    webBuilder.UseUrls(url);
                })
                .Build();
        }

        internal static IFeatsEvaluationClient GivenClient(
            this FeatsEvaluationClientTests tests,
            IFeatsEvaluationConfiguration configuration,
            IEvaluationCache cache,
            IHttpClientFactory httpClientFactory)
        {
            return new FeatsEvaluationClient(
                configuration,
                new TestLogger<FeatsEvaluationClient>(),
                httpClientFactory,
                cache);
        }

        internal static async Task ThenWeGet(this Task<bool> task, bool expected)
        {
            var results = await task;

            results.Should().Be(expected);
        }
    }

    internal class TestNoCacheEvaluationCache : IEvaluationCache
    {
        public Task<bool> isOn(IFeatureEvaluationRequest request, Func<Task<bool>> isOnTask)
        {
            return isOnTask();
        }
    }
    

    internal class TestCacheEvaluationCache : IEvaluationCache
    {
        private readonly bool _expected;

        public TestCacheEvaluationCache(bool expected)
        {
            this._expected = expected;
        }
        
        public Task<bool> isOn(IFeatureEvaluationRequest request, Func<Task<bool>> isOnTask)
        {
            return Task.FromResult(this._expected);
        }
    }
}