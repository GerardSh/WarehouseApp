using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;

namespace WarehouseApp.Web.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected readonly ILogger<T> logger;

        protected BaseController(ILogger<T> logger)
        {
            this.logger = logger;
        }

        protected string? GetUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected IActionResult? ValidateUserIdOrRedirect(string? userId, ref Guid parsedUserId)
        {
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out parsedUserId))
            {
                logger.LogWarning(UnauthorizedAccess);

                TempData["ErrorMessage"] = UserNotFound;
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }

            return null;
        }
    }
}
