



#region inceleme

//using EventStore.Client;
//using System.Text.Json;





//string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";
//var settings = EventStoreClientSettings.Create(connectionString);
//var client = new EventStoreClient(settings);





////while(true)
////{
////    OrderPlacedEvent orderPlacedEvent = new()
////    {
////        OrderId = 1,
////        TotalAmount = 1000
////    };
////    EventData eventData = new(
////    eventId: Uuid.NewUuid(),
////    type: orderPlacedEvent.GetType().Name,
////    data: JsonSerializer.SerializeToUtf8Bytes(orderPlacedEvent));

////    await client.AppendToStreamAsync(
////           streamName: "order-stream",
////              expectedState: StreamState.Any,
////             new[] { eventData
////    });
////}





////var events = client.ReadStreamAsync(
////    streamName: "order-stream",
////    direction: Direction.Forwards,
////    revision: StreamPosition.Start
////    );


////var datas = await events.ToListAsync();





////await client.SubscribeToStreamAsync(
////    streamName: "order-stream",
////    start: FromStream.Start,
////    eventAppeared: async (subscription, @event, cancellationToken) =>
////    {
////        var sss = JsonSerializer.Deserialize<OrderPlacedEvent>(@event.Event.Data.ToArray());
////        Console.Out.WriteLineAsync(JsonSerializer.Serialize(sss));
////    },
////    subscriptionDropped: async (subscription, reason, exception) =>
////    {
////        Console.WriteLine($"Subscription dropped. Reason: {reason}");
////    }
////    );





//Console.ReadLine();


//public class OrderPlacedEvent
//{
//    public int OrderId { get; set; }
//    public int TotalAmount { get; set; }
//}




#endregion




#region MyRegion



using EventStore.Client;
using System.Text.Json;



EventStoreService eventStoreService = new();





await eventStoreService.SubscribeToStreamAsync(
    streamName: "costumer-98765-stream",
    async (ss, re, ct) =>
    {
        var eventType = re.Event.EventType;
        object @event = JsonSerializer.Deserialize(re.Event.Data.ToArray(), Type.GetType(eventType));

        // işlemler
    });























class EventStoreService
{
    EventStoreClientSettings GetEventStoreClientSettings(string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false")
        => EventStoreClientSettings.Create(connectionString);

    EventStoreClient Client { get => new EventStoreClient(GetEventStoreClientSettings()); }



    public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventData)
       => await Client.AppendToStreamAsync(streamName, StreamState.Any, eventData);

    public EventData GenerateEventData(object @event)
        => new EventData(
                       eventId: Uuid.NewUuid(),
                       type: @event.GetType().Name,
                       data: JsonSerializer.SerializeToUtf8Bytes(@event));

    public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared)
        => await Client.SubscribeToStreamAsync(streamName: streamName, start: FromStream.Start, eventAppeared: eventAppeared, subscriptionDropped: (x, y, z) => Console.WriteLine("Disconnected"));



}










class BlanceInfo
{
    public string AccountId { get; set; }
    public int Balance { get; set; }
}

class AccountCreatedEvent
{
    public string AccountId { get; set; }
    public string CustomerId { get; set; }
    public int StartBalance { get; set; }
    public DateTime Date { get; set; }
}

class MoneyDepositedEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}

class MoneyWithDrawnEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}

class MoneyTransferedEvent
{
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}




#endregion



