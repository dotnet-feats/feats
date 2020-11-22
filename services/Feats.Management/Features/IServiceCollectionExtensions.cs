using System;
using Feats.Management.Features.Commands;
using Feats.Management.Features.Events;
using Feats.Management.Features.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Management.Features
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services
            .AddCommands()
            .AddEvents()
            .AddQueries();

            return services;
        }
    }
}
