//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using VariBillWebAPI.Models.DTO;
//using VariBillWebAPI.Services.Interfaces;

//namespace VariBillWebAPI.Controllers;

//[ApiController]
//[Route("api/products")]
//[Authorize]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductService _productService;

//    public ProductsController(IProductService productService)
//    {
//        _productService = productService;
//    }

//    [HttpGet]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    public async Task<ActionResult<IReadOnlyList<ProductResponseDto>>> GetAll()
//    {
//        var products = await _productService.GetAllAsync();
//        return Ok(products);
//    }

//    [HttpGet("{id:guid}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
//    {
//        var product = await _productService.GetByIdAsync(id);
//        return product is null ? NotFound() : Ok(product);
//    }

//    [HttpPost]
//    [ProducesResponseType(StatusCodes.Status201Created)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
//    {
//        try
//        {
//            var created = await _productService.CreateAsync(dto);
//            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
//        }
//        catch (InvalidOperationException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }

//    [HttpPut("{id:guid}")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
//    {
//        try
//        {
//            var updated = await _productService.UpdateAsync(id, dto);
//            return updated ? NoContent() : NotFound();
//        }
//        catch (InvalidOperationException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }

//    [HttpDelete("{id:guid}")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> Delete(Guid id)
//    {
//        var deleted = await _productService.DeleteAsync(id);
//        return deleted ? NoContent() : NotFound();
//    }
//}
