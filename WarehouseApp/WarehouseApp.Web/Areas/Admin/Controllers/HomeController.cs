using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


using WarehouseApp.Web.Controllers;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.RolesConstants;

namespace WarehouseApp.Web.Areas.Admin.Controllers
{
    [Area(AdminArea)]
    [Authorize(Roles = AdminRoleName)]
    public class HomeController : BaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger)
                : base(logger)
        {
        }

        // TODO: Add actions so the user can submit Manager request and the Admin can approve this request
        public IActionResult Index()
        {
            return View();
        }
    }
}
