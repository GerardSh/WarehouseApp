namespace WarehouseApp.Common.OutputMessages
{
    public static class ErrorMessages
    {
        public static class Application
        {
            public const string UserNotFound = "User not found!";
        }

        public static class Warehouse
        {
            public const string RequiredName = "Warehouse name is required!";
            public const string RequiredAddress = "Warehouse address is required!";
            public const string ZeroSize = "Size must be greater than 0 and lower than 100,000!";

            public const string CreationFailure = "Fatal error occurred while creating the warehouse!";
            public const string EditingFailure = "Fatal error occurred while editing the warehouse!";
            public const string DeletionFailure = "Fatal error occurred while deleting the warehouse!";
            public const string CreationSuccess = "The warehouse was successfully created!";
            public const string EditingSuccess = "The warehouse was successfully edited!";
            public const string DeletionSuccess = "The warehouse was successfully deleted!";

            public const string WarehouseDuplicateName = "A warehouse with the same name already exists. Please choose a different name!";
            public const string WarehouseNotFound = "Warehouse not found!";
            public const string NoPermission = "You don't have permission to perform this action!";
            public const string NoPermissionOrWarehouseNotFound = "Warehouse not found or you don't have permission to edit it!";
            public const string AlreadyDeleted = "Warehouse is already deleted!";
        }

        public static class ApplicationBuilderExtensions
        {
            public const string AdminNotFound = "Admin user with email '{0}' not found!";
            public const string CreatingRole = "Error occurred while creating the '{0}' role!";
            public const string AddingUserToRole = "Error occurred while adding the user '{0}' to the '{1}' role!";
            public const string RegisteringRole = "Error occurred while registering '{0}' user!";
        }
    }
}
