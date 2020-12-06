using System;
using EventStore.Client;
using Feats.CQRS.Streams;
using Feats.EventStore.Aggregates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.EventStore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IReadStreamedEvents<FeatureStream>, FeatureStreamEventsReader>();
            services.TryAddSingleton<IReadStreamedEvents<PathStream>, PathStreamEventsReader>();
            services.TryAddSingleton<IEventStoreConfiguration, EventStoreConfiguration>();
            services.TryAddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
            
            // the original client is a singleton, so keeping up with tradition i guess
            services.TryAddSingleton<IEventStoreClient>(provider => {
                var factory = provider.GetRequiredService<IEventStoreClientFactory>();
                
                return factory.Create();
            });


            services.TryAddScoped<IPathsAggregate, PathsAggregate>();
            services.TryAddScoped<IFeaturesAggregate, FeaturesAggregate>();

            return services;
        }
    }
}
