using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL3.Entities;
using PBL3.Models;


namespace PBL3.Controllers
{
    public class HomeController : Controller

    {
    
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Action tr? v? view chính (Index)
        public IActionResult Index()
        {
            return View();
        }

        // Action tr? v? view Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // Action tr? v? view Error (n?u có l?i)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
