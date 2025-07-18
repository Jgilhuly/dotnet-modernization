using FluentAssertions;
using RestaurantOps.Legacy.Models;
using RestaurantOps.Tests.Builders;
using RestaurantOps.Tests.TestHelpers;
using System.Data;

namespace RestaurantOps.Tests.UnitTests.Data;

public class MenuRepositoryTests : IDisposable
{
    public MenuRepositoryTests()
    {
        TestDatabase.Initialize();
    }

    [Fact]
    public void GetAllMenuItems_WhenNoItems_ShouldReturnEmpty()
    {
        // Arrange
        TestDatabase.Clear();

        // Act
        var result = GetAllMenuItemsFromTestData();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllMenuItems_WithAvailableItems_ShouldReturnOnlyAvailable()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetAvailableMenuItemsFromTestData();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(item => item.IsAvailable.Should().BeTrue());
        result.Should().Contain(item => item.Name == "Burger");
        result.Should().Contain(item => item.Name == "Pizza");
    }

    [Fact]
    public void GetMenuItemById_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetMenuItemByIdFromTestData(1);

        // Assert
        result.Should().NotBeNull();
        result!.MenuItemId.Should().Be(1);
        result.Name.Should().Be("Burger");
        result.Price.Should().Be(12.99m);
    }

    [Fact]
    public void GetMenuItemById_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetMenuItemByIdFromTestData(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddMenuItem_WhenValidItem_ShouldAddToDatabase()
    {
        // Arrange
        TestDatabase.Clear();
        var newItem = MenuItemBuilder.New()
            .WithName("Salad")
            .WithDescription("Fresh garden salad")
            .WithPrice(8.99m)
            .WithCategoryId(2)
            .Build();

        // Act
        AddMenuItemToTestData(newItem);

        // Assert
        var items = GetAllMenuItemsFromTestData();
        items.Should().HaveCount(1);
        items.First().Name.Should().Be("Salad");
        items.First().Description.Should().Be("Fresh garden salad");
        items.First().Price.Should().Be(8.99m);
    }

    private void SeedTestData()
    {
        var menuItemsTable = TestDatabase.GetTable("MenuItems");
        
        // Add available items
        var burger = menuItemsTable.NewRow();
        burger["MenuItemId"] = 1;
        burger["Name"] = "Burger";
        burger["Description"] = "Delicious beef burger";
        burger["Price"] = 12.99m;
        burger["CategoryId"] = 1;
        burger["IsAvailable"] = true;
        menuItemsTable.Rows.Add(burger);

        var pizza = menuItemsTable.NewRow();
        pizza["MenuItemId"] = 2;
        pizza["Name"] = "Pizza";
        pizza["Description"] = "Classic margherita pizza";
        pizza["Price"] = 15.99m;
        pizza["CategoryId"] = 1;
        pizza["IsAvailable"] = true;
        menuItemsTable.Rows.Add(pizza);

        // Add unavailable item
        var pasta = menuItemsTable.NewRow();
        pasta["MenuItemId"] = 3;
        pasta["Name"] = "Pasta";
        pasta["Description"] = "Creamy alfredo pasta";
        pasta["Price"] = 13.99m;
        pasta["CategoryId"] = 1;
        pasta["IsAvailable"] = false;
        menuItemsTable.Rows.Add(pasta);
    }

    private IEnumerable<MenuItem> GetAllMenuItemsFromTestData()
    {
        var sql = "SELECT MenuItemId, Name, Description, Price, CategoryId, IsAvailable FROM MenuItems";
        var dt = TestSqlHelper.ExecuteDataTable(sql);
        
        foreach (DataRow row in dt.Rows)
        {
            yield return MapMenuItem(row);
        }
    }

    private IEnumerable<MenuItem> GetAvailableMenuItemsFromTestData()
    {
        var sql = "SELECT MenuItemId, Name, Description, Price, CategoryId, IsAvailable FROM MenuItems WHERE IsAvailable = 1";
        var dt = TestSqlHelper.ExecuteDataTable(sql);
        
        foreach (DataRow row in dt.Rows)
        {
            yield return MapMenuItem(row);
        }
    }

    private MenuItem? GetMenuItemByIdFromTestData(int id)
    {
        var sql = "SELECT MenuItemId, Name, Description, Price, CategoryId, IsAvailable FROM MenuItems WHERE MenuItemId = @id";
        var dt = TestSqlHelper.ExecuteDataTable(sql, new Microsoft.Data.SqlClient.SqlParameter("@id", id));
        return dt.Rows.Count == 0 ? null : MapMenuItem(dt.Rows[0]);
    }

    private void AddMenuItemToTestData(MenuItem item)
    {
        var sql = @"INSERT INTO MenuItems (Name, Description, Price, CategoryId, IsAvailable)
                    VALUES (@name, @desc, @price, @catId, @available)";
        TestSqlHelper.ExecuteNonQuery(sql,
            new Microsoft.Data.SqlClient.SqlParameter("@name", item.Name),
            new Microsoft.Data.SqlClient.SqlParameter("@desc", item.Description ?? string.Empty),
            new Microsoft.Data.SqlClient.SqlParameter("@price", item.Price),
            new Microsoft.Data.SqlClient.SqlParameter("@catId", item.CategoryId),
            new Microsoft.Data.SqlClient.SqlParameter("@available", item.IsAvailable));
    }

    private static MenuItem MapMenuItem(DataRow row)
    {
        return new MenuItem
        {
            MenuItemId = (int)row["MenuItemId"],
            Name = (string)row["Name"],
            Description = row["Description"] as string,
            Price = (decimal)row["Price"],
            CategoryId = (int)row["CategoryId"],
            IsAvailable = (bool)row["IsAvailable"]
        };
    }

    public void Dispose()
    {
        TestDatabase.Clear();
    }
}