using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;

namespace NLayer.Web.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Biz herhangi bir controller içerisinde bir action method yazdığımızda hem kendi klasörüne mesela home klasörüne bakar bulamazsa shared'a bakar. Yani iki yere default olarak bakar.
        public IActionResult Error(ErrorViewModel errorViewModel)
        {
            return View(errorViewModel);
        }
    }
}