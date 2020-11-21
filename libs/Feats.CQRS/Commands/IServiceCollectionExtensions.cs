using System;

using Microsoft.Extensions.DependencyInjection;

namespace Feats.CQRS.Commands
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommandHandler<TCommand>(this IServiceCollection services)
            where TCommandy : ICommand
        {
            services.TryAddSingleton<IHandleCommand<TCommand>>();

            return services;
        }
    }
}