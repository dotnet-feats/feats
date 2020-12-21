using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Feats.CQRS.Streams;

namespace Feats.EventStore
{
    /// Wrapper for client
    public interface IEventStoreClient : IDisposable
    {
        // Summary:
        //     Appends events asynchronously to a stream.
        //
        // Parameters:
        //   expectedState:
        //     The expected EventStore.Client.StreamState of the stream to append to.
        //
        //   eventData:
        //     An System.Collections.Generic.IEnumerable`1 to append to the stream.
        //
        //   configureOperationOptions:
        //     An System.Action`1 to configure the operation's options.
        //
        //   userCredentials:
        //     The EventStore.Client.UserCredentials for the operation.
        //
        //   cancellationToken:
        //     The optional System.Threading.CancellationToken.
        Task<IWriteResult> AppendToStreamAsync(
            IStream stream, 
            StreamState expectedState, 
            IEnumerable<EventData> eventData, 
            Action<EventStoreClientOperationOptions>? configureOperationOptions = null, 
            UserCredentials? userCredentials = null, 
            CancellationToken cancellationToken = default);

        // Summary:
        //     Asynchronously reads all the events from a stream. The result could also be inspected
        //     as a means to avoid handling exceptions as the EventStore.Client.ReadState would
        //     indicate whether or not the stream is readable./>
        //
        // Parameters:
        //   direction:
        //     The EventStore.Client.Direction in which to read.
        //
        //   revision:
        //     The EventStore.Client.StreamRevision to start reading from.
        //
        //   maxCount:
        //     The number of events to read from the stream.
        //
        //   configureOperationOptions:
        //     An System.Action`1 to configure the operation's options.
        //
        //   resolveLinkTos:
        //     Whether to resolve LinkTo events automatically.
        //
        //   userCredentials:
        //     The optional EventStore.Client.UserCredentials to perform operation with.
        //
        //   cancellationToken:
        //     The optional System.Threading.CancellationToken.
        IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(
            IStream stream,
            Direction direction, 
            StreamPosition revision, 
            long maxCount = long.MaxValue, 
            Action<EventStoreClientOperationOptions>? configureOperationOptions = null, 
            bool resolveLinkTos = false, 
            UserCredentials? userCredentials = null, 
            CancellationToken cancellationToken = default);
    }

    internal sealed class DecoratedEventStoreClient : IEventStoreClient
    {
        private readonly EventStoreClient _eventStoreClient;

        public DecoratedEventStoreClient(EventStoreClient eventStoreClient)
        {
            this._eventStoreClient = eventStoreClient;
        }

        public void Dispose()
        {
            this._eventStoreClient.Dispose();
        }

        public Task<IWriteResult> AppendToStreamAsync(
            IStream stream,
            StreamState expectedState, 
            IEnumerable<EventData> eventData, 
            Action<EventStoreClientOperationOptions>? configureOperationOptions = null, 
            UserCredentials? userCredentials = null, 
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.AppendToStreamAsync(
                stream.Name, 
                expectedState,
                eventData, 
                configureOperationOptions,
                userCredentials, 
                cancellationToken);
        }

        public async IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(
            IStream stream,
            Direction direction, 
            StreamPosition revision, 
            long maxCount = long.MaxValue, 
            Action<EventStoreClientOperationOptions>? configureOperationOptions = null, 
            bool resolveLinkTos = false, 
            UserCredentials? userCredentials = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var results = this._eventStoreClient.ReadStreamAsync(
                direction,
                stream.Name,
                revision, 
                maxCount,
                configureOperationOptions,
                resolveLinkTos,
                userCredentials, 
                cancellationToken);
            var state = await results.ReadState;
            
            if (state != ReadState.StreamNotFound)
            {
                // todo, i've killed a kitten here, i'm sorry, but i wanted to avoid reflexion, seriously though, why the internal constructor on ReadStreamResults..
                await foreach (var @event in results) {
                    yield return @event;
                }
            }
        }
    } 
}
