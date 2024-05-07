using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductApplication.Models;
using Shared.Events;
using Shared.Models;
using Shared.Services.Abstractions;

namespace ProductApplication.Controllers
{
    public class ProductsController(IEventStoreService eventStoreService, IMongoDbService mongoDbService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var productCollection = mongoDbService.GetCollection<MongoProduct>("Products");
            var products = await (await productCollection.FindAsync(x => true)).ToListAsync();
            return View(products.Select(x => new Product
            {
                Count = x.Count,
                Id = x.Id,
                IsAvaliable = x.IsAvaliable,
                Price = x.Price,
                ProductName = x.ProductName
            }));
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProduct createProduct)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = createProduct.Count,
                InitialPrice = createProduct.Price,
                IsAvaliable = createProduct.IsAvaliable,
                ProductName = createProduct.ProductName
            };
            await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(createProduct) });
            return RedirectToAction("Index");
        }




        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var productCollection = mongoDbService.GetCollection<MongoProduct>("Products");
            var product = await (await productCollection.FindAsync(x => x.Id == id)).FirstOrDefaultAsync();
            return View(new Product
            {
                Count = product.Count,
                Id = product.Id,
                IsAvaliable = product.IsAvaliable,
                Price = product.Price,
                ProductName = product.ProductName
            });
        }

        [HttpPost]
        public async Task<IActionResult> CountUpdate(Product model)
        {
            var productCollection = mongoDbService.GetCollection<MongoProduct>("Products");
            var product = await (await productCollection.FindAsync(x => x.Id == model.Id)).FirstOrDefaultAsync();
            if (product.Count > model.Count)
            {
                CountDecreasedEvent @Event = new()
                {
                    ProductId = model.Id,
                    DecreamentAmount = model.Count
                };
                await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(@Event) });
            }
            else if (product.Count < model.Count)
            {
                CountIncreasedEvent @Event = new()
                {
                    ProductId = model.Id,
                    IncreamentAmount = model.Count
                };
                await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(@Event) });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PriceUpdate(Product model)
        {
            var productCollection = mongoDbService.GetCollection<MongoProduct>("Products");
            var product = await (await productCollection.FindAsync(x => x.Id == model.Id)).FirstOrDefaultAsync();
            if (product.Price > model.Price)
            {
                PriceDecreasedEvent @Event = new()
                {
                    ProductId = model.Id,
                    DecreamentPrice = model.Count
                };
                await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(@Event) });
            }
            else if (product.Price < model.Price)
            {
                PriceIncreasedEvent @Event = new()
                {
                    ProductId = model.Id,
                    IncreamentPrice = model.Count
                };
                await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(@Event) });
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> IsAvaliableUpdate(Product model)
        {
            var productCollection = mongoDbService.GetCollection<MongoProduct>("Products");
            var product = await (await productCollection.FindAsync(x => x.Id == model.Id)).FirstOrDefaultAsync();
            if (product.IsAvaliable != model.IsAvaliable)
            {
                AvailabilityChangedEvent @Event = new()
                {
                    ProductId = model.Id,
                    IsAvaliable = model.IsAvaliable
                };
                await eventStoreService.AppentToStreamAsync("products-stream", new[] { eventStoreService.GenerateEventData(@Event) });
            }
            return RedirectToAction("Index");
        }



    }
}
