namespace WarehouseApp.Common.Constants
{
    public static class RolesConstants
    {
        public const string AdminRoleName = "Administrator";
        public const string UserRoleName = "User";

        public static readonly IReadOnlyList<string> AllRoles = new List<string>
        {
            AdminRoleName,
            UserRoleName
        }.AsReadOnly();
    }
}
