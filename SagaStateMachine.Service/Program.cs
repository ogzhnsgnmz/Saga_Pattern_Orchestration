using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SagaStateMachine.Service.StateDbContexts;
using SagaStateMachine.Service.StateInstances;
using SagaStateMachine.Service.StateMachines;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
    .EntityFrameworkRepository(options =>
    {
        options.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
        {
            _builder.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer"));
        });
    });
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

var host = builder.Build();
host.Run();