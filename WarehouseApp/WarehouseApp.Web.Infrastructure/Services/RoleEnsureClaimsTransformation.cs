using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WarehouseApp.Data.Models;
using Microsoft.AspNetCore.Authentication;


public class RoleEnsureClaimsTransformation : IClaimsTransformation
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleEnsureClaimsTransformation(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Already has the role? No need to check DB.
        if (principal.IsInRole("User"))
            return principal;

        var user = await _userManager.GetUserAsync(principal);
        if (user != null)
        {
            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                // Ensure the "User" role exists
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
                }

                await _userManager.AddToRoleAsync(user, "User");

                // Optional: Add the claim immediately so it's seen in current request
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim(ClaimTypes.Role, "User"));
            }
        }

        return principal;
    }
}
