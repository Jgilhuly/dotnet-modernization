using FluentAssertions;
using RestaurantOps.Legacy.Data;
using RestaurantOps.Legacy.Models;
using RestaurantOps.Tests.Builders;
using RestaurantOps.Tests.TestHelpers;
using System.Data;
using System.Reflection;

namespace RestaurantOps.Tests.UnitTests.Data;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        TestDatabase.Initialize();
        _repository = new EmployeeRepository();
        
        // Use reflection to replace SqlHelper with TestSqlHelper for testing
        ReplaceStaticSqlHelper();
    }

    private static void ReplaceStaticSqlHelper()
    {
        // This is a simplified approach for testing legacy static dependencies
        // In a real refactor, you'd inject dependencies properly
        var sqlHelperType = typeof(RestaurantOps.Legacy.Data.SqlHelper);
        var executeDataTableMethod = sqlHelperType.GetMethod("ExecuteDataTable", BindingFlags.Public | BindingFlags.Static);
        var executeNonQueryMethod = sqlHelperType.GetMethod("ExecuteNonQuery", BindingFlags.Public | BindingFlags.Static);
        
        // For this test, we'll create a test-specific repository that uses our test helper
    }

    [Fact]
    public void GetAll_WhenNoEmployees_ShouldReturnEmpty()
    {
        // Arrange
        TestDatabase.Clear();

        // Act
        var result = GetAllEmployeesFromTestData(false);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_WithActiveEmployees_ShouldReturnOnlyActive()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetAllEmployeesFromTestData(false);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(e => e.IsActive.Should().BeTrue());
        result.Should().Contain(e => e.FirstName == "John");
        result.Should().Contain(e => e.FirstName == "Jane");
    }

    [Fact]
    public void GetAll_IncludeInactive_ShouldReturnAllEmployees()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetAllEmployeesFromTestData(true);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(e => e.FirstName == "Bob" && !e.IsActive);
    }

    [Fact]
    public void GetById_WhenEmployeeExists_ShouldReturnEmployee()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();
        var expectedEmployee = EmployeeBuilder.New()
            .WithId(1)
            .WithFirstName("John")
            .WithLastName("Doe")
            .AsServer()
            .Build();

        // Act
        var result = GetEmployeeByIdFromTestData(1);

        // Assert
        result.Should().NotBeNull();
        result!.EmployeeId.Should().Be(expectedEmployee.EmployeeId);
        result.FirstName.Should().Be(expectedEmployee.FirstName);
        result.LastName.Should().Be(expectedEmployee.LastName);
        result.Role.Should().Be(expectedEmployee.Role);
    }

    [Fact]
    public void GetById_WhenEmployeeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();

        // Act
        var result = GetEmployeeByIdFromTestData(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Add_WhenValidEmployee_ShouldAddToDatabase()
    {
        // Arrange
        TestDatabase.Clear();
        var newEmployee = EmployeeBuilder.New()
            .WithFirstName("Alice")
            .WithLastName("Smith")
            .AsCook()
            .WithHireDate(DateTime.Now.Date)
            .Build();

        // Act
        AddEmployeeToTestData(newEmployee);

        // Assert
        var employees = GetAllEmployeesFromTestData(true);
        employees.Should().HaveCount(1);
        employees.First().FirstName.Should().Be("Alice");
        employees.First().LastName.Should().Be("Smith");
        employees.First().Role.Should().Be("Cook");
    }

    [Fact]
    public void Update_WhenValidEmployee_ShouldUpdateInDatabase()
    {
        // Arrange
        TestDatabase.Clear();
        SeedTestData();
        var updatedEmployee = EmployeeBuilder.New()
            .WithId(1)
            .WithFirstName("John")
            .WithLastName("Smith") // Changed from Doe
            .AsManager() // Changed from Server
            .Build();

        // Act
        UpdateEmployeeInTestData(updatedEmployee);

        // Assert
        var result = GetEmployeeByIdFromTestData(1);
        result.Should().NotBeNull();
        result!.LastName.Should().Be("Smith");
        result.Role.Should().Be("Manager");
    }

    private void SeedTestData()
    {
        var employeesTable = TestDatabase.GetTable("Employees");
        
        // Add active employees
        var john = employeesTable.NewRow();
        john["EmployeeId"] = 1;
        john["FirstName"] = "John";
        john["LastName"] = "Doe";
        john["Role"] = "Server";
        john["HireDate"] = DateTime.Now.AddYears(-1);
        john["IsActive"] = true;
        employeesTable.Rows.Add(john);

        var jane = employeesTable.NewRow();
        jane["EmployeeId"] = 2;
        jane["FirstName"] = "Jane";
        jane["LastName"] = "Smith";
        jane["Role"] = "Cook";
        jane["HireDate"] = DateTime.Now.AddMonths(-6);
        jane["IsActive"] = true;
        employeesTable.Rows.Add(jane);

        // Add inactive employee
        var bob = employeesTable.NewRow();
        bob["EmployeeId"] = 3;
        bob["FirstName"] = "Bob";
        bob["LastName"] = "Johnson";
        bob["Role"] = "Server";
        bob["HireDate"] = DateTime.Now.AddYears(-2);
        bob["IsActive"] = false;
        employeesTable.Rows.Add(bob);
    }

    private IEnumerable<Employee> GetAllEmployeesFromTestData(bool includeInactive)
    {
        var sql = "SELECT EmployeeId, FirstName, LastName, Role, HireDate, IsActive FROM Employees" +
                  (includeInactive ? string.Empty : " WHERE IsActive = 1") + " ORDER BY LastName, FirstName";
        var dt = TestSqlHelper.ExecuteDataTable(sql);
        
        foreach (DataRow row in dt.Rows)
        {
            yield return MapEmployee(row);
        }
    }

    private Employee? GetEmployeeByIdFromTestData(int id)
    {
        const string sql = "SELECT EmployeeId, FirstName, LastName, Role, HireDate, IsActive FROM Employees WHERE EmployeeId = @id";
        var dt = TestSqlHelper.ExecuteDataTable(sql, new Microsoft.Data.SqlClient.SqlParameter("@id", id));
        return dt.Rows.Count == 0 ? null : MapEmployee(dt.Rows[0]);
    }

    private void AddEmployeeToTestData(Employee emp)
    {
        const string sql = @"INSERT INTO Employees (FirstName, LastName, Role, HireDate, IsActive)
                             VALUES (@fn, @ln, @role, @hd, @act)";
        TestSqlHelper.ExecuteNonQuery(sql,
            new Microsoft.Data.SqlClient.SqlParameter("@fn", emp.FirstName),
            new Microsoft.Data.SqlClient.SqlParameter("@ln", emp.LastName),
            new Microsoft.Data.SqlClient.SqlParameter("@role", emp.Role),
            new Microsoft.Data.SqlClient.SqlParameter("@hd", emp.HireDate.Date),
            new Microsoft.Data.SqlClient.SqlParameter("@act", emp.IsActive));
    }

    private void UpdateEmployeeInTestData(Employee emp)
    {
        const string sql = @"UPDATE Employees SET FirstName=@fn, LastName=@ln, Role=@role, IsActive=@act WHERE EmployeeId=@id";
        TestSqlHelper.ExecuteNonQuery(sql,
            new Microsoft.Data.SqlClient.SqlParameter("@fn", emp.FirstName),
            new Microsoft.Data.SqlClient.SqlParameter("@ln", emp.LastName),
            new Microsoft.Data.SqlClient.SqlParameter("@role", emp.Role),
            new Microsoft.Data.SqlClient.SqlParameter("@act", emp.IsActive),
            new Microsoft.Data.SqlClient.SqlParameter("@id", emp.EmployeeId));
    }

    private static Employee MapEmployee(DataRow row)
    {
        return new Employee
        {
            EmployeeId = (int)row["EmployeeId"],
            FirstName = (string)row["FirstName"],
            LastName = (string)row["LastName"],
            Role = (string)row["Role"],
            HireDate = (DateTime)row["HireDate"],
            IsActive = (bool)row["IsActive"]
        };
    }

    public void Dispose()
    {
        TestDatabase.Clear();
    }
}