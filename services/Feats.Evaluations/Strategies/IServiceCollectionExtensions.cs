using System;
using Feats.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Evaluations.Strategies
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddStrategies(this IServiceCollection services)
        {
            services.TryAddSingleton<IEvaluateStrategy<IsOnStrategy>, IsOnStrategyEvaluator>();
            services.TryAddSingleton<IStrategyEvaluatorFactory, StrategyEvaluatorFactory>();
            services.TryAddSingleton<IStrategySettingsSerializer, StrategySettingsSerializer>();
            services.TryAddScoped<IValuesExtractor, ValuesExtractor>();
            return services;
        }
    }
}