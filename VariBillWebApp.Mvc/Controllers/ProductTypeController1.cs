//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using VariBillWebApp.Mvc.Models.ViewModels;
//using VariBillWebApp.Mvc.Services.Abstractions;

//namespace VariBillWebApp.Mvc.Controllers;

///// <summary>
///// Product type management (server-rendered views).
///// </summary>
//[Authorize]
//public class ProductTypeController : Controller
//{
//    private readonly IProductTypeService _productTypeService;
//    private readonly ILogger<ProductTypeController> _logger;

//    public ProductTypeController(
//        IProductTypeService productTypeService,
//        ILogger<ProductTypeController> logger)
//    {
//        _productTypeService = productTypeService;
//        _logger = logger;
//    }

//    public async Task<IActionResult> Index()
//    {
//        var types = await _productTypeService.GetAllProductTypesAsync();
//        return View(types);
//    }

//    [HttpGet]
//    public IActionResult Create() => View();

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(CreateProductTypeModel model)
//    {
//        if (!ModelState.IsValid)
//            return View(model);

//        var result = await _productTypeService.CreateProductTypeAsync(model);
//        if (result == null)
//        {
//            ModelState.AddModelError("", "Failed to create product type.");
//            return View(model);
//        }

//        TempData["Success"] = $"Product type '{model.Name}' created successfully!";
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpGet]
//    public async Task<IActionResult> Edit(Guid id)
//    {
//        var type = await _productTypeService.GetProductTypeByIdAsync(id);
//        if (type == null)
//        {
//            TempData["Error"] = $"Product type with ID {id} not found.";
//            return RedirectToAction(nameof(Index));
//        }

//        var model = new UpdateProductTypeModel
//        {
//            Name = type.Name,
//            Description = type.Description
//        };
//        return View(model);
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Edit(Guid id, UpdateProductTypeModel model)
//    {
//        if (!ModelState.IsValid)
//            return View(model);

//        var success = await _productTypeService.UpdateProductTypeAsync(id, model);
//        if (!success)
//        {
//            ModelState.AddModelError("", "Failed to update product type.");
//            return View(model);
//        }

//        TempData["Success"] = $"Product type '{model.Name}' updated successfully!";
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpGet]
//    public async Task<IActionResult> Delete(Guid id)
//    {
//        var type = await _productTypeService.GetProductTypeByIdAsync(id);
//        if (type == null)
//        {
//            TempData["Error"] = $"Product type with ID {id} not found.";
//            return RedirectToAction(nameof(Index));
//        }
//        return View(type);
//    }

//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    [Authorize(Policy = "RequireAdminRole")]
//    public async Task<IActionResult> DeleteConfirmed(Guid id)
//    {
//        var (success, errorMessage) = await _productTypeService.DeleteProductTypeAsync(id);
//        if (!success && errorMessage != null)
//        {
//            TempData["Error"] = errorMessage;
//            return RedirectToAction(nameof(Index));
//        }
//        if (!success)
//        {
//            TempData["Error"] = $"Product type with ID {id} not found.";
//            return RedirectToAction(nameof(Index));
//        }

//        TempData["Success"] = "Product type deleted successfully!";
//        return RedirectToAction(nameof(Index));
//    }
//}


