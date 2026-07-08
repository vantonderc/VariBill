using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services.Abstractions;


namespace VariBillWebAPI.Controllers;

/// <summary>
/// API controller for product type management.
/// </summary>
[ApiController]
[Route("api/producttypes")]
[Authorize]
public class ProductTypesController : ControllerBase
{
    private readonly IProductTypeService _productTypeService;
    private readonly ILogger<ProductTypesController> _logger;

    public ProductTypesController(IProductTypeService productTypeService, ILogger<ProductTypesController> logger)
    {
        _productTypeService = productTypeService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductTypeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductTypeResponseDto>>> GetAll()
    {
        var types = await _productTypeService.GetAllAsync();
        return Ok(types);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductTypeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductTypeResponseDto>> GetById(Guid id)
    {
        var type = await _productTypeService.GetByIdAsync(id);
        return type is null ? NotFound() : Ok(type);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductTypeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductTypeResponseDto>> Create([FromBody] CreateProductTypeDto dto)
    {
        try
        {
            var created = await _productTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductTypeDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updated = await _productTypeService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (success, errorMessage) = await _productTypeService.DeleteAsync(id);

        if (!success && errorMessage is not null)
            return Conflict(new { error = errorMessage });

        if (!success)
            return NotFound();

        return NoContent();
    }
}



