using System;
using System.Collections.Generic;
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
        Task<IWriteResult> AppendToStreamAsync(IStream stream, StreamState expectedState, IEnumerable<EventData> eventData, Action<EventStoreClientOperationOptions>? configureOperationOptions = null, UserCredentials? userCredentials = null, CancellationToken cancellationToken = default);

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
        
        //
        // Summary:
        //     Subscribes to a stream from a checkpoint. This is exclusive of.
        //
        // Parameters:
        //   start:
        //     A EventStore.Client.Position (exclusive of) to start the subscription from.
        //
        //   eventAppeared:
        //     A Task invoked and awaited when a new event is received over the subscription.
        //
        //   configureOperationOptions:
        //     An System.Action`1 to configure the operation's options.
        //
        //   resolveLinkTos:
        //     Whether to resolve LinkTo events automatically.
        //
        //   subscriptionDropped:
        //     An action invoked if the subscription is dropped.
        //
        //   userCredentials:
        //     The optional user credentials to perform operation with.
        //
        //   cancellationToken:
        //     The optional System.Threading.CancellationToken.
        Task<StreamSubscription> SubscribeToStreamAsync(
            IStream stream, 
            StreamPosition start, 
            Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared, 
            bool resolveLinkTos = false, 
            Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null, 
            Action<EventStoreClientOperationOptions>? configureOperationOptions = null, 
            UserCredentials? userCredentials = null, 
            CancellationToken cancellationToken = default);
    }

    public sealed class DecoratedEventStoreClient : IEventStoreClient
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
            StreamRevision expectedRevision, 
            IEnumerable<EventData> eventData, 
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            UserCredentials userCredentials = null, 
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.AppendToStreamAsync(
                stream.Name, 
                expectedRevision,
                eventData, 
                configureOperationOptions,
                userCredentials, 
                cancellationToken);
        }

        public Task<IWriteResult> AppendToStreamAsync(
            IStream stream,
            StreamState expectedState, 
            IEnumerable<EventData> eventData, 
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            UserCredentials userCredentials = null, 
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

        public Task<StreamMetadataResult> GetStreamMetadataAsync(
            IStream stream,
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            UserCredentials userCredentials = null, 
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.GetStreamMetadataAsync(
                stream.Name, 
                configureOperationOptions,
                userCredentials, 
                cancellationToken);
        }

        public IAsyncEnumerable<ResolvedEvent> ReadAllAsync(
            Direction direction, 
            Position position, 
            long maxCount = long.MaxValue, 
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            bool resolveLinkTos = false, 
            UserCredentials userCredentials = null, 
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.ReadAllAsync(
                direction,
                position,
                maxCount,
                configureOperationOptions,
                resolveLinkTos,
                userCredentials, 
                cancellationToken);
        }

        public async IAsyncEnumerable<ResolvedEvent> ReadStreamAsync(
            IStream stream,
            Direction direction, 
            StreamPosition revision, 
            long maxCount = long.MaxValue, 
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            bool resolveLinkTos = false, 
            UserCredentials userCredentials = null, 
            CancellationToken cancellationToken = default)
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

            // todo, i've killed a kitten here, i'm sorry, but i wanted to avoid reflexion, seriously though, why the internal constructor on ReadStreamResults..
            await foreach (var @event in results) {
                yield return @event;
            }
        }

        public Task<StreamSubscription> SubscribeToStreamAsync(
            IStream stream,
            Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared, 
            bool resolveLinkTos = false, 
            Action<StreamSubscription, SubscriptionDroppedReason, Exception> subscriptionDropped = null,
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            UserCredentials userCredentials = null, 
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.SubscribeToStreamAsync(
                stream.Name,
                eventAppeared, 
                resolveLinkTos,
                subscriptionDropped,
                configureOperationOptions,
                userCredentials, 
                cancellationToken);
        }

        public Task<StreamSubscription> SubscribeToStreamAsync(
            IStream stream,
            StreamPosition start, 
            Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
            bool resolveLinkTos = false,
            Action<StreamSubscription, SubscriptionDroppedReason, Exception> subscriptionDropped = null, 
            Action<EventStoreClientOperationOptions> configureOperationOptions = null, 
            UserCredentials userCredentials = null,
            CancellationToken cancellationToken = default)
        {
            return this._eventStoreClient.SubscribeToStreamAsync(
                stream.Name,
                start,
                eventAppeared, 
                resolveLinkTos,
                subscriptionDropped,
                configureOperationOptions,
                userCredentials, 
                cancellationToken);
        }
    } 
}
