using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Tests.Builders;

public class OrderBuilder
{
    private Order _order;

    public OrderBuilder()
    {
        _order = new Order
        {
            OrderId = 1,
            TableId = 1,
            CreatedAt = DateTime.Now,
            Status = "Open"
        };
    }

    public static OrderBuilder New() => new();

    public OrderBuilder WithId(int id)
    {
        _order.OrderId = id;
        return this;
    }

    public OrderBuilder WithTableId(int tableId)
    {
        _order.TableId = tableId;
        return this;
    }

    public OrderBuilder WithCreatedAt(DateTime createdAt)
    {
        _order.CreatedAt = createdAt;
        return this;
    }

    public OrderBuilder WithClosedAt(DateTime? closedAt)
    {
        _order.ClosedAt = closedAt;
        return this;
    }

    public OrderBuilder WithStatus(string status)
    {
        _order.Status = status;
        return this;
    }

    public OrderBuilder AsOpen() => WithStatus("Open");
    public OrderBuilder AsClosed() => WithStatus("Closed");
    public OrderBuilder AsCancelled() => WithStatus("Cancelled");

    public Order Build() => _order;

    public static implicit operator Order(OrderBuilder builder) => builder.Build();
}