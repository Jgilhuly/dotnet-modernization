using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Tests.Builders;

public class MenuItemBuilder
{
    private MenuItem _menuItem;

    public MenuItemBuilder()
    {
        _menuItem = new MenuItem
        {
            MenuItemId = 1,
            Name = "Burger",
            Description = "Delicious beef burger",
            Price = 12.99m,
            CategoryId = 1,
            IsAvailable = true
        };
    }

    public static MenuItemBuilder New() => new();

    public MenuItemBuilder WithId(int id)
    {
        _menuItem.MenuItemId = id;
        return this;
    }

    public MenuItemBuilder WithName(string name)
    {
        _menuItem.Name = name;
        return this;
    }

    public MenuItemBuilder WithDescription(string description)
    {
        _menuItem.Description = description;
        return this;
    }

    public MenuItemBuilder WithPrice(decimal price)
    {
        _menuItem.Price = price;
        return this;
    }

    public MenuItemBuilder WithCategoryId(int categoryId)
    {
        _menuItem.CategoryId = categoryId;
        return this;
    }

    public MenuItemBuilder WithIsAvailable(bool isAvailable)
    {
        _menuItem.IsAvailable = isAvailable;
        return this;
    }

    public MenuItemBuilder AsUnavailable() => WithIsAvailable(false);

    public MenuItem Build() => _menuItem;

    public static implicit operator MenuItem(MenuItemBuilder builder) => builder.Build();
}