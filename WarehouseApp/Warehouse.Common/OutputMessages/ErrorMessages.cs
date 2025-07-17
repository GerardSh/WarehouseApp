namespace WarehouseApp.Common.OutputMessages
{
    public static class ErrorMessages
    {
        public static class Application
        {
            public const string UserNotFound = "User not found.";
        }

        public static class UserManager
        {
            public const string UserOrRoleNotFound = "User or role not found.";
            public const string FailedToAssignRole = "Failed to assign the selected role to the user.";
            public const string FailedToRemoveRole = "Failed to remove the selected role from the user.";
            public const string FailedToDeleteUser = "Failed to delete the selected user.";
        }

        public static class Warehouse
        {
            public const string RequiredName = "Warehouse name is required.";
            public const string RequiredAddress = "Warehouse address is required.";
            public const string ZeroSize = "Size must be greater than 0 and lower than 100,000.";

            public const string CreationFailure = "Fatal error occurred while creating the warehouse.";
            public const string EditingFailure = "Fatal error occurred while editing the warehouse.";
            public const string DeletionFailure = "Fatal error occurred while deleting the warehouse.";
            public const string CreationSuccess = "The warehouse was successfully created.";
            public const string EditingSuccess = "The warehouse was successfully edited.";
            public const string DeletionSuccess = "The warehouse was successfully deleted.";

            public const string WarehouseDuplicateName = "A warehouse with the same name already exists. Please choose a different name.";
            public const string WarehouseNotFound = "Warehouse not found.";
            public const string NoPermission = "You don't have permission to perform this action.";
            public const string NoPermissionOrWarehouseNotFound = "Warehouse not found or you don't have permission to edit it.";
            public const string AlreadyDeleted = "Warehouse is already deleted.";
        }

        public static class ImportInvoice
        {
            public const string InvoiceNumberRequired = "Invoice number is required.";
            public const string InvoiceNumberMaxLength = "Invoice number must be at least 3 characters.";
            public const string InvoiceNumberMinLength = "Invoice number cannot be longer than 20 characters.";
            public const string DateRequired = "Date is required.";
            public const string DuplicateInvoice = "An invoice with this number already exists.";

            public const string CreationFailure = "Fatal error occurred while creating the Invoice.";
            public const string EditingFailure = "Fatal error occurred while editing the Invoice.";
            public const string InvoiceNotFoundOrAccessDeniced = "Invoice not found or access denied.";
        }

        public static class Supplier
        {
            public const string SupplierNameRequired = "Supplier name is required.";
            public const string SupplierNameMinLength = "Supplier name must be at least 3 characters.";
            public const string SupplierNameMaxLength = "Supplier name cannot be longer than 150 characters.";

            public const string SupplierAddressRequired = "Supplier address is required.";
            public const string SupplierAddressMinLength = "Supplier address must be at least 5 characters.";
            public const string SupplierAddressMaxLength = "Supplier address cannot be longer than 150 characters.";

            public const string SupplierPhoneNumberMinLength = "Phone number must be at least 10 characters.";
            public const string SupplierPhoneNumberMaxLength = "Phone number cannot be longer than 30 characters.";
            public const string SupplierPhoneNumberInvalid = "Invalid phone number format.";

            public const string SupplierEmailMinLength = "Email must be at least 5 characters.";
            public const string SupplierEmailMaxLength = "Email cannot be longer than 100 characters.";
            public const string SupplierEmailInvalid = "Invalid email address format.";
        }

        public static class ImportInvoiceDetail
        {
            public const string QuantityRequired = "Quantity is required.";
            public const string QuantityRange = "Quantity must be greater than 0.";
            public const string PriceRange = "Price must be a positive number.";

            public const string CreationFailure = "Failed to create or update ImportInvoiceDetail.";
        }

        public static class Product
        {
            public const string ProductNameRequired = "Product name is required.";
            public const string ProductNameMinLength = "Product name must be at least 3 characters.";
            public const string ProductNameMaxLength = "Product name cannot be longer than 255 characters.";
            public const string ProductDescriptionMaxLength = "Product description cannot be longer than 1000 characters.";
            public const string ProductDescriptionMinLength = "Product description must be at least 5 characters.";
            public const string ProductDuplicate = "Cannot have more than one product with the same name and category.";
            public const string CreationFailure = "Failed to create or update product.";
        }

        public static class Category
        {
            public const string CategoryNameRequired = "Category name is required.";
            public const string CategoryNameMinLength = "Category name must be at least 3 characters.";
            public const string CategoryNameMaxLength = "Category name cannot be longer than 255 characters.";
            public const string CategoryDescriptionMaxLength = "Category description cannot be longer than 1000 characters.";
            public const string CategoryDescriptionMinLength = "Category description must be at least 5 characters.";

            public const string CreationFailure = "Failed to create or update category.";
        }

        public static class Client
        {
            public const string CreationFailure = "Failed to create or update client.";
        }

        public static class ApplicationBuilderExtensions
        {
            public const string AdminNotFound = "Admin user with email '{0}' not found.";
            public const string CreatingRole = "Error occurred while creating the '{0}' role.";
            public const string AddingUserToRole = "Error occurred while adding the user '{0}' to the '{1}' role.";
            public const string RegisteringRole = "Error occurred while registering '{0}' user.";
        }
    }
}
