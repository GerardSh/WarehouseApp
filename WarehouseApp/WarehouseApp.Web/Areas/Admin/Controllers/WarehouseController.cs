using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using WarehouseApp.Web.Controllers;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.RolesConstants;

namespace WarehouseApp.Web.Areas.Admin.Controllers
{
    [Area(AdminArea)]
    [Authorize(Roles = AdminRoleName)]
    public class WarehouseController : BaseController<WarehouseController>
    {
        public WarehouseController(ILogger<WarehouseController> logger)
                : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View("~/Views/Shared/_UnderConstruction.cshtml");
        }
    }
}