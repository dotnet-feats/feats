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
            services.TryAddSingleton<IEvaluateStrategy<IsInListStrategy>, IsInListStrategyEvaluator>();
            services.TryAddSingleton<IEvaluateStrategy<IsGreaterThanStrategy>, IsGreaterThanStrategyEvaluator>();
            services.TryAddSingleton<IEvaluateStrategy<IsLowerThanStrategy>, IsLowerThanStrategyEvaluator>();
            
            services.TryAddSingleton<IStrategyEvaluatorFactory, StrategyEvaluatorFactory>();
            services.TryAddSingleton<IStrategySettingsSerializer, StrategySettingsSerializer>();
            services.TryAddScoped<IValuesExtractor, ValuesExtractor>();
            return services;
        }
    }
}