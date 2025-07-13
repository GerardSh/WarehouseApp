namespace WarehouseApp.Web.ViewModels.Admin.UserManagement
{
    public class AllUsersWithRolesSearchFilterViewModel
    {
        public string? SearchQuery { get; set; }

        public int? CurrentPage { get; set; } = 1;

        public int? EntitiesPerPage { get; set; } = 5;

        public int TotalItemsBeforePagination { get; set; }

        public int? TotalPages { get; set; }

        public int TotalUsers { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; } = null!;

        public IEnumerable<string> AllRoles { get; set; } = null!;
    }
}
