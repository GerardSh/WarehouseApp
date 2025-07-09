namespace WarehouseApp.Web.ViewModels.Admin.UserManagement
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        public string? Email { get; set; }

        public IEnumerable<string> Roles { get; set; } = null!;
    }
}
