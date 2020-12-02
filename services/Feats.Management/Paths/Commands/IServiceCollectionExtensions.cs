using System;
using Microsoft.Extensions.DependencyInjection;

namespace Feats.Management.Paths.Commands
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            return services;
        }
    }
}
