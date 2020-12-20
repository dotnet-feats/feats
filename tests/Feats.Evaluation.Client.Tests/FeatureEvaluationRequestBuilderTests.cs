using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class FeatureEvaluationRequestBuilderTests
    {
        [Test]
        public void GivenBuilder_WhenAddingNameAndPath_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsInNamedList_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var listItem = "roger";
            var listName = "roger's stuff";
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsInList(listName, listItem)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(listName) && _.Value.Equals(listItem))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsInDefaultList_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var listItem = "roger";
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsInList(listItem)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(StrategySettings.List) && _.Value.Equals(listItem))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsGreater_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var number = 12345.456;
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsGreaterThan(number)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(StrategySettings.GreaterThan) && _.Value.Equals(number.ToInvariantString()))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsLower_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var number = 12345.456;
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsLowerThan(number)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(StrategySettings.LowerThan) && _.Value.Equals(number.ToInvariantString()))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsAfter_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var time = DateTimeOffset.Now;
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsAfter(time)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(StrategySettings.After) && _.Value.Equals(time.ToIsoStopBuggingMeFormat()))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenAddingNameAndPathAndWithIsBefore_ThenICanBuild()
        {
            var name = "name";
            var path = "path";
            var time = DateTimeOffset.Now;
            
            var request = this.GivenBuilder()
                .WithName(name)
                .WithPath(path)
                .WithIsBefore(time)
                .Build();

            request.Should().NotBeNull();
            request.Name.Should().Be(name);
            request.Path.Should().Be(path);
            request.Strategies.Any(_ =>
                    _.Key.Equals(StrategySettings.Before) && _.Value.Equals(time.ToIsoStopBuggingMeFormat()))
                .Should().BeTrue();
        }
        
        [Test]
        public void GivenBuilder_WhenMissingName_ThenWeThrow()
        {
            var path = "path";
            var requestFunc = new Func<IFeatureEvaluationRequest>(() => this.GivenBuilder()
                .WithPath(path)
                .Build());
            requestFunc.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void GivenBuilder_WhenMissingPath_ThenWeThrow()
        {
            var name = "name";
            var requestFunc = new Func<IFeatureEvaluationRequest>(() => this.GivenBuilder()
                .WithName(name)
                .Build());
            requestFunc.Should().Throw<ArgumentNullException>();
        }
    }

    internal static class FeatureEvaluationRequestBuilderTestsExtension
    {
        internal static IFeatureEvaluationRequestBuilder GivenBuilder(this FeatureEvaluationRequestBuilderTests tests)
        {
            return new FeatureEvaluationRequestBuilder();
        }
    }
}