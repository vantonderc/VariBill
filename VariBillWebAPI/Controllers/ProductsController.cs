using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VariBillWebAPI.Exceptions;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services.Abstractions;

namespace VariBillWebAPI.Controllers;

/// <summary>
/// API controller for product management.
/// </summary>
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponseDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Retrieves a product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var created = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (VariBillWebAPI.Exceptions.ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updated = await _productService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (VariBillWebAPI.Exceptions.ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a product (requires Admin role).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
