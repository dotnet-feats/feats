using System;
using Feats.Domain.Validations;
using Microsoft.Extensions.Configuration;

namespace Feats.Management.EventStoreSetups
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

            this.HostName = configuration.GetValue<string>("feats.eventstore.hostname", "localhost");
            this.Protocol = configuration.GetValue<string>("feats.eventstore.protocol", "http");
            this.Username = configuration.GetValue<string>("feats.eventstore.protocol", "admin");
            this.Password = configuration.GetValue<string>("feats.eventstore.protocol", "changeit");
            this.IsClusterModeEnabled = configuration.GetValue<bool>("feats.eventstore.isCluster", false);
        }

        public string HostName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Protocol { get; set; }

        public bool IsClusterModeEnabled { get; set; }
    }
}
