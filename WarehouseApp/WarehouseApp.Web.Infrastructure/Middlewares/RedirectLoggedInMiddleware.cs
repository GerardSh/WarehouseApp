using Microsoft.AspNetCore.Http;

namespace WarehouseApp.Web.Infrastructure.Middlewares
{
    public class RedirectLoggedInMiddleware
    {
        private readonly RequestDelegate next;

        public RedirectLoggedInMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var userAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

            if (userAuthenticated &&
                (path.StartsWithSegments("/Identity/Account/Login") ||
                 path.StartsWithSegments("/Identity/Account/Register")))
            {
                context.Response.Redirect("/Warehouse/Index");
                return;
            }

            await next(context);
        }
    }
}
