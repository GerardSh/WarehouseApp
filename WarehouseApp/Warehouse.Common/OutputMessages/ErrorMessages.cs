namespace WarehouseApp.Common.OutputMessages
{
    public static class ErrorMessages
    {
        public static class Warehouse
        {
            public const string RequiredName = "Warehouse name is required.";
            public const string RequiredAddress = "Warehouse address is required.";
            public const string ZeroSize = "Size must be greater than 0 and lower than 100,000.";

            public const string CreationFailure = "Fatal error occurred while creating the warehouse!";
        }

        public static class ApplicationBuilderExtensions
        {
            public const string AdminNotFound = "Admin user with email '{0}' not found.";
            public const string CreatingRole = "Error occurred while creating the '{0}' role!";
            public const string AddingUserToRole = "Error occurred while adding the user '{0}' to the '{1}' role!";
            public const string RegisteringRole = "Error occurred while registering '{0}' user!";
        }
    }
}
