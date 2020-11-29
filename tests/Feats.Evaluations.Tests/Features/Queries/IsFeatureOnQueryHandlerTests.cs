using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Common.Tests;
using Feats.Domain;
using Feats.Evaluations.Features;
using Feats.Evaluations.Features.Exceptions;
using Feats.Evaluations.Features.Queries;
using Feats.Evaluations.Strategies;
using Feats.Evaluations.Tests.Features.TestExtensions;
using Feats.Evaluations.Tests.Strategies.TestExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluations.Tests.Features.Queries
{
    public class IsFeatureOnQueryHandlerTests : TestBase
    {
        [Test]
        public async Task GivenOffStrategy_WhenEvaluating_ThenIGetOff()
        {
            var query = new IsFeatureOnQuery()
            {
                Name = "name",
                Path = "path/",
            };
            var features = new List<IFeature> {
                new Feature 
                {
                    Name = "name",
                    Path = "path/",
                    State = FeatureState.Published,
                    Strategies = new Dictionary<string, string> 
                    {
                        { "one", "two" },
                    },
                }
            };

            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);
            
            var strategyEvaluator = this.GivenIStrategyEvaluatorFactory()
                .WithOff();

            await this
                .GivenHandler(featuresAggregate.Object, strategyEvaluator.Object)
                .WhenHandling(query)
                .ThenIGetIsOff();
        }

        [Test]
        public async Task GivenOnStrategy_WhenEvaluating_ThenIGetOn()
        {
            var query = new IsFeatureOnQuery()
            {
                Name = "name",
                Path = "path/",
            };
            var features = new List<IFeature> {
                new Feature 
                {
                    Name = "name",
                    Path = "path/",
                    State = FeatureState.Published,
                    Strategies = new Dictionary<string, string> 
                    {
                        { "one", "two" },
                    },
                }
            };

            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);
            
            var strategyEvaluator = this.GivenIStrategyEvaluatorFactory()
                .WithOn();

            await this
                .GivenHandler(featuresAggregate.Object, strategyEvaluator.Object)
                .WhenHandling(query)
                .ThenIGetIsOn();
        }

        [Test]
        public async Task GivenOnStrategy_WhenEvaluatingDraftFeature_ThenIThrow()
        {
            var query = new IsFeatureOnQuery()
            {
                Name = "name",
                Path = "path/",
            };
            var features = new List<IFeature> {
                new Feature 
                {
                    Name = "name",
                    Path = "path/",
                    State = FeatureState.Draft,
                    Strategies = new Dictionary<string, string> 
                    {
                        { "one", "two" },
                    },
                }
            };

            var featuresAggregate = this.GivenIFeaturesAggregate()
                .WithFeatures(features);
            
            var strategyEvaluator = this.GivenIStrategyEvaluatorFactory()
                .WithOn();

            await this
                .GivenHandler(featuresAggregate.Object, strategyEvaluator.Object)
                .WhenHandling(query)
                .ThenExceptionIsThrown<FeatureNotPublishedExeption>();
        }
    }

    public static class IsFeatureOnQueryHandlerTestsExtensions
    {
        public static IsFeatureOnQueryHandler GivenHandler(
            this IsFeatureOnQueryHandlerTests tests,
            IFeaturesAggregate featuresAggregate,
            IStrategyEvaluatorFactory strategyEvaluator)
        {
            return new IsFeatureOnQueryHandler(featuresAggregate, strategyEvaluator);
        }

        public static Func<Task<bool>> WhenHandling(
            this IsFeatureOnQueryHandler handler,
            IsFeatureOnQuery query)
        {
            return () => handler.Handle(query);
        }

        public static async Task ThenIGetIsOn(
            this Func<Task<bool>> handlerFunc)
        {
            var results = await handlerFunc();

            results.Should().BeTrue();
        }

        public static async Task ThenIGetIsOff(
            this Func<Task<bool>> handlerFunc)
        {
            var results = await handlerFunc();

            results.Should().BeFalse();
        }
    }
}