

using ProductEventHandlerService.Services;
using Shared.Services;
using Shared.Services.Abstractions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<EventStoreServiceConsumer>();

builder.Services.AddSingleton<IEventStoreService, EventStoreService>();

builder.Services.AddSingleton<IMongoDbService, MongoDbService>();


var host = builder.Build();
host.Run();
