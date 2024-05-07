using EventStore.Client;
using Shared.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class EventStoreService : IEventStoreService
    {
        EventStoreClientSettings GetEventStoreClientSettings(string connectionSettings = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false") => EventStoreClientSettings.Create(connectionSettings);

        EventStoreClient Client { get => new(GetEventStoreClientSettings()); }

        public async Task AppentToStreamAsync(string streamName, IEnumerable<EventData> eventData)
        => await Client.AppendToStreamAsync(streamName: streamName, expectedState: StreamState.Any, eventData: eventData);

        public EventData GenerateEventData(object @event)
        => new EventData(
                    eventId: Uuid.NewUuid(),
                    type: @event.GetType().Name,
                    data: JsonSerializer.SerializeToUtf8Bytes(@event)
                   );

        public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared)
        => await Client.SubscribeToStreamAsync(streamName: streamName, start: FromStream.Start, eventAppeared: eventAppeared, subscriptionDropped: (streamSubscription, subscriptionDroppedReason, exception) => Console.WriteLine("Disconnected"));









        public async Task<ResolvedEvent> SubscribeToStreamAsync2(string streamName, CancellationToken ct)
        {
            await using var subscription = Client.SubscribeToStream(
                                                        streamName,
                                                        FromStream.Start,
                                                        cancellationToken: ct);
            await foreach (var message in subscription.Messages.WithCancellation(ct))
            {
                switch (message)
                {
                    case StreamMessage.Event(var evnt):
                         return evnt;
                    default:
                        return default;
                }
            }
            return default;
        }









    }
}
