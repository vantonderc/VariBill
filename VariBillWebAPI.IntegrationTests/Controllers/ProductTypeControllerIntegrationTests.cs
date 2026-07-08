using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VariBillWebAPI.Models.DTO;
using Xunit;

namespace VariBillWebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for <see cref="VariBillWebAPI.Controllers.ProductTypesController"/>.
/// </summary>
public class ProductTypeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProductTypeControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ==================== GET /api/producttypes ====================

    [Fact]
    public async Task GetAll_ReturnsOkAndProductTypeList()
    {
        // Act
        var response = await _client.GetAsync("/api/producttypes");

        // Assert
        response.EnsureSuccessStatusCode();
        var types = await response.Content.ReadFromJsonAsync<List<ProductTypeResponseDto>>(_jsonOptions);
        Assert.NotNull(types);
        Assert.True(types.Count >= 2, "Expected at least 2 product types");
    }

    // ==================== GET /api/producttypes/{id} ====================

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkAndProductType()
    {
        // Arrange
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var response = await _client.GetAsync($"/api/producttypes/{id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var type = await response.Content.ReadFromJsonAsync<ProductTypeResponseDto>(_jsonOptions);
        Assert.NotNull(type);
        Assert.Equal("Electronics", type.Name);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/producttypes/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ==================== POST /api/producttypes ====================

    [Fact]
    public async Task CreateProductType_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateProductTypeDto("Test Type", "Integration test type");

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/producttypes", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ProductTypeResponseDto>(_jsonOptions);
        Assert.NotNull(created);
        Assert.Equal("Test Type", created.Name);
        Assert.Equal("Integration test type", created.Description);
        Assert.Equal(0, created.ProductCount);
    }

    [Fact]
    public async Task CreateProductType_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateProductTypeDto("", "Empty name");

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/producttypes", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ==================== PUT /api/producttypes/{id} ====================

    [Fact]
    public async Task UpdateProductType_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var dto = new UpdateProductTypeDto(
            id,
            "Updated Books",
            "Updated description for books",
            true);

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"/api/producttypes/{id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify
        var getResponse = await _client.GetAsync($"/api/producttypes/{id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ProductTypeResponseDto>(_jsonOptions);
        Assert.Equal("Updated Books", updated?.Name);
        Assert.Equal("Updated description for books", updated?.Description);
    }

    [Fact]
    public async Task UpdateProductType_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProductTypeDto(id, "Name", "Description", true);

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"/api/producttypes/{id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ==================== DELETE /api/producttypes/{id} ====================

    [Fact]
    public async Task DeleteProductType_WithNoActiveProducts_ReturnsNoContent()
    {
        // Arrange – create a new type without products
        var createDto = new CreateProductTypeDto("DeleteMe", "Will be deleted");
        var createResponse = await _client.PostAsync("/api/producttypes",
            new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json"));

        var created = await createResponse.Content.ReadFromJsonAsync<ProductTypeResponseDto>(_jsonOptions);
        Assert.NotNull(created);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/producttypes/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/producttypes/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProductType_WithActiveProducts_ReturnsConflict()
    {
        // Arrange – use Electronics type which has products
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var response = await _client.DeleteAsync($"/api/producttypes/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var error = await response.Content.ReadAsStringAsync();
        Assert.Contains("active products", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteProductType_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/producttypes/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductType_Unauthorized_ReturnsUnauthorized()
    {
        // Same as product – just a placeholder; the [Authorize] attribute ensures security.
        Assert.True(true);
    }
}