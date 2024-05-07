using EventStore.Client;

namespace Shared.Services.Abstractions
{
    public interface IEventStoreService
    {
        Task AppentToStreamAsync(string streamName, IEnumerable<EventData> eventData);
        EventData GenerateEventData(object @event);
        Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared);
        Task<ResolvedEvent> SubscribeToStreamAsync2(string streamName, CancellationToken ct);
    }
}
