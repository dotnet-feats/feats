using System;
using Feats.Domain.Validations;
using Microsoft.Extensions.Configuration;

namespace Feats.EventStore
{
    public interface IEventStoreConfiguration
    {
        string HostName { get; }

        string Protocol { get; }

        string Username { get; set; }
        
        string Password { get; set; }

        bool IsClusterModeEnabled { get; }
    }

    public class EventStoreConfiguration : IEventStoreConfiguration
    {
        public EventStoreConfiguration(IConfiguration configuration)
        {
            configuration.Required(nameof(configuration));
            var section = configuration.GetSection("feats:eventstore");

            this.HostName = section.GetValue<string>("hostname", "localhost");
            this.Protocol = section.GetValue<string>("protocol", "http");
            this.Username = section.GetValue<string>("username", "admin");
            this.Password = section.GetValue<string>("password", "changeit");
            this.IsClusterModeEnabled = section.GetValue<bool>("isCluster", false);
        }

        public string HostName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Protocol { get; set; }

        public bool IsClusterModeEnabled { get; set; }
    }
}
