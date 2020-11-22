using System;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.EventStoreSetups
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IEventStoreConfiguration, EventStoreConfiguration>();
            services.TryAddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
            
            services.TryAddSingleton<IEventStoreClient>(provider => {
                var factory = provider.GetRequiredService<IEventStoreClientFactory>();
                
                return factory.Create();
            });
            services.AddEventStoreClient();

            return services;
        }
    }
}
