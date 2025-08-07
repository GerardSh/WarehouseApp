using WarehouseApp.Data.Models.Enums;

namespace WarehouseApp.Data.Models
{
    public class AdminRequest
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public string Reason { get; set; } = null!;

        public DateTime RequestedAt { get; set; }

        public AdminRequestStatus Status { get; set; } = 0;
    }
}
