using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        protected bool ValidateUserId(string? userId, ref Guid parsedUserId)
        {
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out parsedUserId))
            {
                logger.LogWarning("Unauthorized access attempt with invalid user ID.");
                return false;
            }

            return true; ;
        }
    }
}
