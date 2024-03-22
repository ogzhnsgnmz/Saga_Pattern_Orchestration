using MassTransit;
using Order.API.Context;
using Shared.OrderEvents;

namespace Order.API.Consumers;

public class OrderFailedEventConsumer(OrderDbContext orderDbContext) : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        Models.Order order = await orderDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = Enums.OrderStatus.Fail;
            await orderDbContext.SaveChangesAsync();
        }
    }
}
