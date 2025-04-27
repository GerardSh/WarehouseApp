using Microsoft.AspNetCore.Mvc;

namespace WarehouseApp.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsGuidValid(string? id, ref Guid cinemaGuid)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            bool isGuidValid = Guid.TryParse(id, out cinemaGuid);
            if (!isGuidValid)
            {
                return false;
            }

            return true;
        }
    }
}
