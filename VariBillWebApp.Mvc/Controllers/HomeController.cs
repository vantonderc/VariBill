using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VariBillWebApp.Mvc.Models.ViewModels;
using VariBillWebApp.Mvc.Services.Abstractions;

namespace VariBillWebApp.Mvc.Controllers;

/// <summary>
/// Home page and dashboard.
/// </summary>
[Route("")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("legacyproducts")]
    public async Task<IActionResult> LegacyProducts()
    {
      //  var products = await _productService.GetAllProductsAsync();
        return View();
    }

    [HttpGet("privacy")]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}







//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using VariBillWebApp.Mvc.Models;
//using VariBillWebApp.Mvc.Models.ViewModels;

////TODO:NB-> make use of Razor Components or components in general especially for Blazor...

//namespace VariBillWebApp.Mvc.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        public HomeController(ILogger<HomeController> logger)
//        {
//            _logger = logger;
//        }

//       // [ValidateAntiForgeryToken]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
