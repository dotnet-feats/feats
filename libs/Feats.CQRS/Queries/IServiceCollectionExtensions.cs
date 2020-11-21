using System;

using Microsoft.Extensions.DependencyInjection;

namespace Feats.CQRS.Queries
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterQueryHandler<TQuery, TResult>(this IServiceCollection services)
            where TQuery : IQuery<TResult>
            where TResult : class
        {
            services.TryAddSingleton<IHandleQuery<TQuery, TResult>>();

            return services;
        }
    }
}