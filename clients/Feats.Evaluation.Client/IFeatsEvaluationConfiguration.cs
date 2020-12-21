using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Feats.Evaluation.Client
{
    public interface IFeatsEvaluationConfiguration
    {
        Uri Host { get; }
        
        TimeSpan RequestTimeout { get;}
        
        TimeSpan CacheTimeout { get;}
    }

    internal sealed class FeatsEvaluationConfiguration : IFeatsEvaluationConfiguration
    {

        public FeatsEvaluationConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration), 
                    "A base configuration was not provided for FeatsEvaluationConfiguration.");
            }

            var featsSection = configuration.GetSection("feats");

            if (!featsSection.GetChildren().Any())
            {
                throw new ArgumentNullException(
                    nameof(configuration), 
                    "A configuration was not provided for FeatsEvaluationConfiguration : feats section.");
            }

            var host = featsSection.GetValue<string>("host");
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(
                    nameof(configuration), 
                    "A configuration was not provided for FeatsEvaluationConfiguration : feats:host section.");
            }

            try
            {
                var builder = new UriBuilder(host);
                this.Host = builder.Uri;
            }
            catch (UriFormatException formatException)
            {
                throw new InvalidOperationException(
                    "The provided feats:host could not form a proper URI.", 
                    formatException);
            }

            this.RequestTimeout = TimeSpan.FromSeconds(featsSection.GetValue<int>("request_timeout_in_seconds", 300));
            this.CacheTimeout = TimeSpan.FromSeconds(featsSection.GetValue<int>("cache_timeout_in_seconds", 30));
        }

        public Uri Host { get; }
        
        public TimeSpan RequestTimeout { get; }
        
        public TimeSpan CacheTimeout { get; }
    }
}