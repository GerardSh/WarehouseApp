using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Web.Controllers;
using static WarehouseApp.Common.Constants.ApplicationConstants;
using static WarehouseApp.Common.Constants.RolesConstants;

namespace WarehouseApp.Web.Areas.Admin.Controllers
{
    [Area(AdminArea)]
    [Authorize(Roles = AdminRoleName)]
    public class InvoiceController : BaseController<InvoiceController>
    {
        public InvoiceController(ILogger<InvoiceController> logger)
                : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View("~/Views/Shared/_UnderConstruction.cshtml");
        }
    }
}
