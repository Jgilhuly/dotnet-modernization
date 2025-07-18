using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantOps.Legacy.Controllers;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Tests.UnitTests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);
    }

    [Fact]
    public void Index_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ShouldReturnViewResultWithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeOfType<ErrorViewModel>();
        var model = viewResult.Model as ErrorViewModel;
        model!.RequestId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Error_ShouldHaveCorrectCacheSettings()
    {
        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ViewResult>();
        
        // Check that the action has the ResponseCache attribute
        var method = _controller.GetType().GetMethod(nameof(HomeController.Error));
        var cacheAttribute = method!.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;
        
        cacheAttribute.Should().NotBeNull();
        cacheAttribute!.Duration.Should().Be(0);
        cacheAttribute.Location.Should().Be(ResponseCacheLocation.None);
        cacheAttribute.NoStore.Should().BeTrue();
    }
}