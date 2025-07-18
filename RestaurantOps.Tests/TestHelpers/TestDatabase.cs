using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RestaurantOps.Tests.TestHelpers;

public static class TestDatabase
{
    private static readonly Dictionary<string, DataTable> _inMemoryTables = new();
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized) return;
        
        SetupInMemoryTables();
        _isInitialized = true;
    }

    public static void Clear()
    {
        foreach (var table in _inMemoryTables.Values)
        {
            table.Clear();
        }
    }

    public static void Reset()
    {
        _inMemoryTables.Clear();
        _isInitialized = false;
        Initialize();
    }

    private static void SetupInMemoryTables()
    {
        // Create Employees table structure
        var employeesTable = new DataTable("Employees");
        employeesTable.Columns.Add("EmployeeId", typeof(int));
        employeesTable.Columns.Add("FirstName", typeof(string));
        employeesTable.Columns.Add("LastName", typeof(string));
        employeesTable.Columns.Add("Role", typeof(string));
        employeesTable.Columns.Add("HireDate", typeof(DateTime));
        employeesTable.Columns.Add("IsActive", typeof(bool));
        _inMemoryTables["Employees"] = employeesTable;

        // Create MenuItems table structure  
        var menuItemsTable = new DataTable("MenuItems");
        menuItemsTable.Columns.Add("MenuItemId", typeof(int));
        menuItemsTable.Columns.Add("Name", typeof(string));
        menuItemsTable.Columns.Add("Description", typeof(string));
        menuItemsTable.Columns.Add("Price", typeof(decimal));
        menuItemsTable.Columns.Add("CategoryId", typeof(int));
        menuItemsTable.Columns.Add("IsAvailable", typeof(bool));
        _inMemoryTables["MenuItems"] = menuItemsTable;

        // Create Orders table structure
        var ordersTable = new DataTable("Orders");
        ordersTable.Columns.Add("OrderId", typeof(int));
        ordersTable.Columns.Add("TableId", typeof(int));
        ordersTable.Columns.Add("OrderDate", typeof(DateTime));
        ordersTable.Columns.Add("Status", typeof(string));
        ordersTable.Columns.Add("Total", typeof(decimal));
        _inMemoryTables["Orders"] = ordersTable;

        // Create Tables table structure
        var tablesTable = new DataTable("Tables");
        tablesTable.Columns.Add("TableId", typeof(int));
        tablesTable.Columns.Add("Number", typeof(int));
        tablesTable.Columns.Add("Seats", typeof(int));
        tablesTable.Columns.Add("IsOccupied", typeof(bool));
        _inMemoryTables["Tables"] = tablesTable;
    }

    public static DataTable GetTable(string tableName)
    {
        return _inMemoryTables.TryGetValue(tableName, out var table) ? table : new DataTable();
    }

    public static void AddData(string tableName, DataRow row)
    {
        if (_inMemoryTables.TryGetValue(tableName, out var table))
        {
            table.ImportRow(row);
        }
    }
}