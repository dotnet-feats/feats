using System;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Management.Paths.Events
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEvents(this IServiceCollection services)
        {
            return services;
        }
    }
}
