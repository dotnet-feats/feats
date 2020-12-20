using System;
using Feats.CQRS.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feats.Management.Features.Commands
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.TryAddScoped<IHandleCommand<CreateFeatureCommand>, CreateFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<PublishFeatureCommand>, PublishFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<AssignIsBeforeStrategyToFeatureCommand>, AssignIsBeforeStrategyToFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<AssignIsAfterStrategyToFeatureCommand>, AssignIsAfterStrategyToFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<AssignIsOnStrategyToFeatureCommand>, AssignIsOnStrategyToFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<AssignIsInListStrategyToFeatureCommand>, AssignIsInListStrategyToFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<UnAssignStrategyCommand>, UnAssignStrategyCommandHandler>();
            services.TryAddScoped<IHandleCommand<UpdateFeatureCommand>, UpdateFeatureCommandHandler>();
            services.TryAddScoped<IHandleCommand<ArchiveFeatureCommand>, ArchiveFeatureCommandHandler>();

            return services;
        }
    }
}
