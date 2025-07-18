# RestaurantOps Testing Infrastructure

This project contains comprehensive unit and integration tests for the Restaurant Operations system.

## 🏗️ Infrastructure Components

### Testing Framework
- **xUnit** - Primary testing framework
- **FluentAssertions** - Readable assertion library
- **Moq** - Mocking framework for dependencies
- **WebApplicationFactory** - Integration testing for ASP.NET Core

### Test Organization
```
RestaurantOps.Tests/
├── UnitTests/           # Unit tests for individual components
│   ├── Data/           # Repository tests
│   └── Controllers/    # Controller tests
├── IntegrationTests/   # End-to-end API tests
├── Builders/          # Test data builders
├── TestHelpers/       # Test infrastructure and utilities
└── Scripts/           # Test execution scripts
```

## 🧪 Test Data Builders

Use fluent builders to create test data easily:

```csharp
var employee = EmployeeBuilder.New()
    .WithFirstName("John")
    .WithLastName("Doe")
    .AsServer()
    .WithHireDate(DateTime.Now.AddYears(-1))
    .Build();

var menuItem = MenuItemBuilder.New()
    .WithName("Burger")
    .WithPrice(12.99m)
    .AsUnavailable()
    .Build();
```

## 🏃‍♂️ Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "EmployeeRepositoryTests"

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Using Scripts
```bash
# Linux/Mac
chmod +x Scripts/run-tests.sh
./Scripts/run-tests.sh

# Windows PowerShell
.\Scripts\run-tests.ps1
```

## 📊 Code Coverage

### Configuration
- Coverage settings in `coverlet.runsettings`
- Includes: `RestaurantOps.Legacy` project
- Excludes: Test projects, migrations, generated code

### Reports
After running tests with coverage:
- **HTML Report**: `TestResults/coverage-html/index.html`
- **Cobertura XML**: `TestResults/coverage.cobertura.xml`
- **Console Summary**: Displayed after test run

### Targets
- **Line Coverage**: 80%+
- **Branch Coverage**: 75%+
- **Method Coverage**: 85%+

## 🧩 Test Patterns

### Unit Tests
```csharp
[Fact]
public void GetById_WhenEmployeeExists_ShouldReturnEmployee()
{
    // Arrange
    TestDatabase.Clear();
    SeedTestData();
    
    // Act
    var result = repository.GetById(1);
    
    // Assert
    result.Should().NotBeNull();
    result.FirstName.Should().Be("John");
}
```

### Integration Tests
```csharp
[Fact]
public async Task Get_Employees_ShouldReturnSuccessStatusCode()
{
    // Act
    var response = await client.GetAsync("/api/employees");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Mock Usage
```csharp
var mockLogger = new Mock<ILogger<HomeController>>();
var controller = new HomeController(mockLogger.Object);

// Verify logger was called
mockLogger.Verify(
    x => x.Log(LogLevel.Error, It.IsAny<EventId>(), 
               It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), 
               It.IsAny<Func<It.IsAnyType, Exception, string>>()),
    Times.Once);
```

## 🗄️ Test Database

### In-Memory Strategy
- `TestDatabase` class provides in-memory DataTable storage
- `TestSqlHelper` simulates SQL operations without real database
- Each test class resets state in constructor/dispose

### Seeding Data
```csharp
private void SeedTestData()
{
    var table = TestDatabase.GetTable("Employees");
    var row = table.NewRow();
    row["EmployeeId"] = 1;
    row["FirstName"] = "John";
    table.Rows.Add(row);
}
```

## 🎯 Best Practices

### Test Naming
- `MethodName_Scenario_ExpectedOutcome`
- Example: `GetById_WhenEmployeeNotFound_ShouldReturnNull`

### Arrange-Act-Assert
```csharp
[Fact]
public void TestMethod()
{
    // Arrange - Set up test data and expectations
    var input = "test data";
    
    // Act - Execute the method under test
    var result = methodUnderTest(input);
    
    // Assert - Verify the outcome
    result.Should().Be("expected");
}
```

### Test Independence
- Each test should be independent
- Use `IDisposable` to clean up after tests
- Avoid shared state between tests

### Readable Assertions
```csharp
// Good
result.Should().NotBeNull();
result.Should().HaveCount(3);
result.Should().Contain(x => x.Name == "John");

// Better than
Assert.NotNull(result);
Assert.Equal(3, result.Count);
Assert.True(result.Any(x => x.Name == "John"));
```

## 🚀 Continuous Integration

### GitHub Actions (Example)
```yaml
- name: Test
  run: dotnet test --no-build --collect:"XPlat Code Coverage"
  
- name: Upload coverage
  uses: codecov/codecov-action@v3
  with:
    files: TestResults/coverage.cobertura.xml
```

## 🔧 Troubleshooting

### Common Issues
1. **Tests not found**: Ensure test methods are `public` and marked with `[Fact]`
2. **Coverage not generated**: Check `coverlet.runsettings` file location
3. **Integration tests fail**: Verify `WebApplicationFactory` configuration

### Debug Tests
```bash
# Run single test with debugging
dotnet test --filter "TestMethodName" --logger "console;verbosity=detailed"
```