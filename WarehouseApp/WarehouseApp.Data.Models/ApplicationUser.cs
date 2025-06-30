using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data.Models
{
    [Comment("Extended Identity user with application-specific properties")]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Id = Guid.NewGuid();
        }

        [Comment("Full name of the user")]
        public string? FullName { get; set; }

        public virtual ICollection<ApplicationUserWarehouse> Warehouses { get; set; }
            = new HashSet<ApplicationUserWarehouse>();
    }
}
