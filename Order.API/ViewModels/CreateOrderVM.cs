using Order.API.Models;

namespace Order.API.ViewModels;

public class CreateOrderVM
{
    public int BuyerId { get; set; }
    public List<OrderItem> OrderItems { get; set; }

}
