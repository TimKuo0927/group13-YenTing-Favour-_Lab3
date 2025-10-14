using System.Diagnostics;
using group_13_YenTing_Favour__Lab_3.Models;
using Microsoft.AspNetCore.Mvc;

namespace group_13_YenTing_Favour__Lab_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            var userName = User.Identity.IsAuthenticated ? User.Identity.Name : "guests";
            return View((object)userName); // cast to object so MVC treats it as a model
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
}
