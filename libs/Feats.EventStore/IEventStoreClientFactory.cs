using System;
using System.Net;
using System.Net.Http;
using EventStore.Client;

namespace Feats.EventStore
{
    public interface IEventStoreClientFactory
    {
        IEventStoreClient Create();
    }

    public sealed class EventStoreClientFactory : IEventStoreClientFactory
    {
        private readonly IEventStoreConfiguration _eventStoreConfiguration;

        public EventStoreClientFactory(IEventStoreConfiguration eventStoreConfiguration)
        {
            this._eventStoreConfiguration = eventStoreConfiguration;
        }

        public IEventStoreClient Create()
        {
            if (this._eventStoreConfiguration.IsClusterModeEnabled)
            {
                var clusterSettings = new EventStoreClientSettings {
	                Interceptors = new[] {new MissingInterceptor()},
                    ConnectivitySettings =
                    {
		                Address = new Uri($"{this._eventStoreConfiguration.Protocol}://{this._eventStoreConfiguration.HostName}:2113"),
                        DnsGossipSeeds = new[]
                        {
                            new DnsEndPoint(this._eventStoreConfiguration.HostName, 1114),
                            new DnsEndPoint(this._eventStoreConfiguration.HostName, 2114),
                            new DnsEndPoint(this._eventStoreConfiguration.HostName, 3114),
                        }
                    },
                    DefaultCredentials = new UserCredentials(
                        this._eventStoreConfiguration.Username, 
                        this._eventStoreConfiguration.Password),
                    CreateHttpMessageHandler = () =>
                        new SocketsHttpHandler {
                            SslOptions = {
                                RemoteCertificateValidationCallback = delegate {
                                    return true;
                                }
                            }
                        },
                };

                return new DecoratedEventStoreClient(new EventStoreClient(clusterSettings));
            }

            var settings = new EventStoreClientSettings {
                ConnectionName = "Management",
                Interceptors = new[] {new MissingInterceptor()},
                ConnectivitySettings =
                {
		            Address = new Uri($"{this._eventStoreConfiguration.Protocol}://{this._eventStoreConfiguration.HostName}:2113")
                },
                DefaultCredentials = new UserCredentials(
                    this._eventStoreConfiguration.Username, 
                    this._eventStoreConfiguration.Password),
                CreateHttpMessageHandler = () =>
                    new SocketsHttpHandler {
                        SslOptions = {
                            RemoteCertificateValidationCallback = delegate {
                                return true;
                            }
                        }
                    },
            };

            return new DecoratedEventStoreClient(new EventStoreClient(settings));
        }
    }
}
