using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WarehouseApp.Web.Controllers;
using WarehouseApp.Web.ViewModels;

namespace WarehouseApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Warehouse");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode = null)
        {
            int code = statusCode ?? HttpContext.Response.StatusCode;

            if (code == 401 || code == 403)
            {
                return this.View("Error403");
            }
            else if (code == 404)
            {
                return this.View("Error404");
            }
            else if (code == 500)
            {
                return this.View("Error500");
            }

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            };

            return this.View(model);
        }
    }
}
