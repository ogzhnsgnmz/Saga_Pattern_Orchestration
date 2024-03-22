using MassTransit;
using MongoDB.Driver;
using Shared.Settings;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<StockRollbackMessageConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue, e => e.ConfigureConsumer<StockRollbackMessageConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

using var scope = builder.Services.BuildServiceProvider().CreateScope();

var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDbService>();

if(!await (await mongoDbService.GetCollection<Stock.API.Models.Stock>()
    .FindAsync(x => true)).AnyAsync())
{
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new(){ ProductId = 1, Count = 100 });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new(){ ProductId = 2, Count = 200 });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new(){ ProductId = 3, Count = 300 });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new(){ ProductId = 4, Count = 400 });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new(){ ProductId = 5, Count = 500 });
}

if(mongoDbService != null)
    
app.Run();