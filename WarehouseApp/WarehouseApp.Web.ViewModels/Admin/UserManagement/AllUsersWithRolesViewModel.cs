namespace WarehouseApp.Web.ViewModels.Admin.UserManagement
{
    public class AllUsersWithRolesViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; } = null!;

        public IEnumerable<string> AllRoles { get; set; } = null!;
    }
}
