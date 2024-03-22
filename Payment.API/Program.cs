using MassTransit;
using Payment.API.Consumers;
using Shared.Settings;
using Stock.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentStartedEventConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.ReceiveEndpoint(RabbitMQSettings.Payment_StartedEventQueue, e => e.ConfigureConsumer<PaymentStartedEventConsumer>(context));

        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

app.Run();