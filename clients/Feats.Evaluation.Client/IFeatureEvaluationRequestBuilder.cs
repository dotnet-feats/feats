using System;
using System.Collections.Generic;
using System.Linq;

namespace Feats.Evaluation.Client
{
    public interface IFeatureEvaluationRequestBuilder
    {
        IFeatureEvaluationRequestBuilder WithName(string featureName);
        
        IFeatureEvaluationRequestBuilder WithPath(string featurePath);
        
        IFeatureEvaluationRequestBuilder WithIsInList(string listName, string value);

        IFeatureEvaluationRequestBuilder WithIsInList(string value);
        
        IFeatureEvaluationRequestBuilder WithIsBefore(DateTimeOffset givenDate);
        
        IFeatureEvaluationRequestBuilder WithIsAfter(DateTimeOffset givenDate);
        
        IFeatureEvaluationRequestBuilder WithIsGreaterThan(double givenValue);
        
        IFeatureEvaluationRequestBuilder WithIsLowerThan(double givenValue);

        IFeatureEvaluationRequest Build();
    }
    
    internal sealed class FeatureEvaluationRequestBuilder : IFeatureEvaluationRequestBuilder
    {
        private readonly IFeatureEvaluationRequest _request;
        
        internal FeatureEvaluationRequestBuilder()
        {
            this._request = new FeatureEvaluationRequest();
        }
        
        private FeatureEvaluationRequestBuilder(IFeatureEvaluationRequest request)
        {
            this._request = request;
        }
        
        public IFeatureEvaluationRequestBuilder WithName(string featureName)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = featureName,
                Path = this._request.Path,
                Strategies = this._request.Strategies
            });
        }

        public IFeatureEvaluationRequestBuilder WithPath(string featurePath)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = featurePath,
                Strategies = this._request.Strategies
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsInList(string listName, string value)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(listName, value))
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsInList(string value)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(StrategySettings.List, value))
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsBefore(DateTimeOffset givenDate)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(
                    StrategySettings.Before, 
                    givenDate.ToIsoStopBuggingMeFormat()))
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsAfter(DateTimeOffset givenDate)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(
                    StrategySettings.After, 
                    givenDate.ToIsoStopBuggingMeFormat()))
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsGreaterThan(double givenValue)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(
                    StrategySettings.GreaterThan, 
                    givenValue.ToInvariantString()))
            });
        }

        public IFeatureEvaluationRequestBuilder WithIsLowerThan(double givenValue)
        {
            return new FeatureEvaluationRequestBuilder(new FeatureEvaluationRequest
            {
                Name = this._request.Name,
                Path = this._request.Path,
                Strategies = this._request.Strategies.Append(new KeyValuePair<string, string>(
                    StrategySettings.LowerThan, 
                    givenValue.ToInvariantString()))
            });
        }

        public IFeatureEvaluationRequest Build()
        {
            this._request.Name.Required(nameof(this._request.Name));
            this._request.Path.Required(nameof(this._request.Name));

            return this._request;
        }
    }
}