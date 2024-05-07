using MongoDB.Driver;
using Shared.Events;
using Shared.Models;
using Shared.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductEventHandlerService.Services
{
    public class EventStoreServiceConsumer(IEventStoreService eventStoreService, IMongoDbService mongoDbService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await eventStoreService.SubscribeToStreamAsync(streamName: "products-stream", async (streamSubscription, resolvedEvent, cancellationToken) =>
            {
                string eventType = resolvedEvent.Event.EventType;
                object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType));

                var productsCollection = mongoDbService.GetCollection<MongoProduct>("Products");

                Shared.Models.MongoProduct? product = null;
                switch (@event)
                {
                    case NewProductAddedEvent e:
                        var hasProduct = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).AnyAsync();
                        if (!hasProduct)
                        {
                            await productsCollection.InsertOneAsync(new()
                            {
                                Id = e.ProductId,
                                Count = e.InitialCount,
                                Price = e.InitialPrice,
                                IsAvaliable = e.IsAvaliable,
                                ProductName = e.ProductName
                            });
                        }
                        break;
                    case CountDecreasedEvent e:
                        product = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Count -= e.DecreamentAmount;
                            await productsCollection.ReplaceOneAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case CountIncreasedEvent e:
                        product = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Count += e.IncreamentAmount;
                            await productsCollection.ReplaceOneAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case PriceDecreasedEvent e:
                        product = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Price -= e.DecreamentPrice;
                            await productsCollection.ReplaceOneAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case PriceIncreasedEvent e:
                        product = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.Price += e.IncreamentPrice;
                            await productsCollection.ReplaceOneAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    case AvailabilityChangedEvent e:
                        product = await (await productsCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.IsAvaliable = e.IsAvaliable;
                            await productsCollection.ReplaceOneAsync(p => p.Id == e.ProductId, product);
                        }
                        break;
                    default:
                        break;
                }


            });
        }
    }
}
