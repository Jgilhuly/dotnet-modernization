using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Tests.Builders;

public class RestaurantTableBuilder
{
    private RestaurantTable _table;

    public RestaurantTableBuilder()
    {
        _table = new RestaurantTable
        {
            TableId = 1,
            Name = "Table 1",
            Seats = 4,
            IsOccupied = false
        };
    }

    public static RestaurantTableBuilder New() => new();

    public RestaurantTableBuilder WithId(int id)
    {
        _table.TableId = id;
        return this;
    }

    public RestaurantTableBuilder WithName(string name)
    {
        _table.Name = name;
        return this;
    }

    public RestaurantTableBuilder WithSeats(int seats)
    {
        _table.Seats = seats;
        return this;
    }

    public RestaurantTableBuilder WithIsOccupied(bool isOccupied)
    {
        _table.IsOccupied = isOccupied;
        return this;
    }

    public RestaurantTableBuilder AsOccupied() => WithIsOccupied(true);
    public RestaurantTableBuilder AsAvailable() => WithIsOccupied(false);

    public RestaurantTable Build() => _table;

    public static implicit operator RestaurantTable(RestaurantTableBuilder builder) => builder.Build();
}