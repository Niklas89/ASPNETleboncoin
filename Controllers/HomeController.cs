using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lebonanimal.Datas;
using lebonanimal.Models;

namespace lebonanimal.Controllers;

public class HomeController : Controller
{
    private readonly DbConnect _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(DbConnect context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(_context.Products.ToList());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}