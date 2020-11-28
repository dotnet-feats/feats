using System;
using Feats.CQRS.Streams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.Paths.Events
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEvents(this IServiceCollection services)
        {
            services.TryAddSingleton<IReadStreamedEvents<PathStream>, PathStreamEventsReader>();

            return services;
        }
    }
}
