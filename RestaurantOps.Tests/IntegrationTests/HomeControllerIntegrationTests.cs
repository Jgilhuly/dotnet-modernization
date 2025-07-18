using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace RestaurantOps.Tests.IntegrationTests;

public class HomeControllerIntegrationTests : IClassFixture<WebApplicationTestFactory<Program>>
{
    private readonly WebApplicationTestFactory<Program> _factory;
    private readonly HttpClient _client;

    public HomeControllerIntegrationTests(WebApplicationTestFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Index_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Index_ShouldReturnHtmlContent()
    {
        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("<!DOCTYPE html");
        content.Should().Contain("<html");
    }

    [Fact]
    public async Task Privacy_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/Home/Privacy");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Privacy_ShouldReturnHtmlContent()
    {
        // Act
        var response = await _client.GetAsync("/Home/Privacy");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("<!DOCTYPE html");
        content.Should().Contain("<html");
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home")]
    [InlineData("/Home/Index")]
    [InlineData("/Home/Privacy")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().Should().StartWith("text/html");
    }
}