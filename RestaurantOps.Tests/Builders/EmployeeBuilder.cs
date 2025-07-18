using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Tests.Builders;

public class EmployeeBuilder
{
    private Employee _employee;

    public EmployeeBuilder()
    {
        _employee = new Employee
        {
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Role = "Server",
            HireDate = DateTime.Now.AddYears(-1),
            IsActive = true
        };
    }

    public static EmployeeBuilder New() => new();

    public EmployeeBuilder WithId(int id)
    {
        _employee.EmployeeId = id;
        return this;
    }

    public EmployeeBuilder WithFirstName(string firstName)
    {
        _employee.FirstName = firstName;
        return this;
    }

    public EmployeeBuilder WithLastName(string lastName)
    {
        _employee.LastName = lastName;
        return this;
    }

    public EmployeeBuilder WithRole(string role)
    {
        _employee.Role = role;
        return this;
    }

    public EmployeeBuilder WithHireDate(DateTime hireDate)
    {
        _employee.HireDate = hireDate;
        return this;
    }

    public EmployeeBuilder WithIsActive(bool isActive)
    {
        _employee.IsActive = isActive;
        return this;
    }

    public EmployeeBuilder AsServer() => WithRole("Server");
    public EmployeeBuilder AsCook() => WithRole("Cook");
    public EmployeeBuilder AsManager() => WithRole("Manager");
    public EmployeeBuilder AsInactive() => WithIsActive(false);

    public Employee Build() => _employee;

    public static implicit operator Employee(EmployeeBuilder builder) => builder.Build();
}