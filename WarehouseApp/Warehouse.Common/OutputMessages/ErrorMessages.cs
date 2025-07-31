namespace WarehouseApp.Common.OutputMessages
{
    public static class ErrorMessages
    {
        public static class Application
        {
            public const string UserNotFound = "User not found.";
            public const string UnauthorizedAccess = "Unauthorized access attempt with invalid user ID.";
        }

        public static class UserManager
        {
            public const string RoleNotFound = "Role not found.";
            public const string FailedToAssignRole = "Failed to assign the selected role to the user.";
            public const string FailedToRemoveRole = "Failed to remove the selected role from the user.";
            public const string FailedToDeleteUser = "Failed to delete the selected user.";
            public const string FailedToMarkWarehouse = "Failed to mark warehouse as deleted.";
            public const string DeletionFailure = "Fatal error occurred while deleting the user.";
            public const string RemoveRoleFailure = "Fatal error occurred while removing the user role.";
            public const string AssignRoleFailure = "Fatal error occurred while assigning the user role.";
            public const string GetAllUsersFailure = "Fatal error occurred while getting all users.";
            public const string UserExistsFailure = "Fatal error occurred while checking the user.";
        }

        public static class Warehouse
        {
            public const string RequiredName = "Warehouse name is required.";
            public const string RequiredAddress = "Warehouse address is required.";
            public const string ZeroSize = "Size must be greater than 0 and lower than 100,000.";

            public const string RetrievingFailure = "Fatal error occurred while retrieving the warehouse.";
            public const string CreationFailure = "Fatal error occurred while creating the warehouse.";
            public const string GetModelFailure = "Fatal error occurred while getting the warehouse.";
            public const string EditingFailure = "Fatal error occurred while editing the warehouse.";
            public const string DeletionFailure = "Fatal error occurred while deleting the warehouse.";

            public const string WarehouseDuplicateName = "A warehouse with the same name already exists. Please choose a different name.";
            public const string WarehouseNotFound = "Warehouse not found.";
            public const string NoPermission = "You don't have permission to perform this action.";
            public const string NoPermissionOrWarehouseNotFound = "Warehouse not found or you don't have permission to edit it.";
            public const string AlreadyDeleted = "Warehouse is already deleted.";
        }

        public static class ImportInvoice
        {
            public const string InvoiceNumberRequired = "Invoice number is required.";
            public const string InvoiceNumberMaxLength = "Invoice number cannot be longer than 20 characters.";
            public const string InvoiceNumberMinLength = "Invoice number must be at least 3 characters.";
            public const string DateRequired = "Date is required.";
            public const string DuplicateInvoice = "An invoice with this number already exists.";
            public const string NoPermissionOrImportInvoiceNotFound = "Invoice not found or access denied.";

            public const string CreationFailure = "Fatal error occurred while creating the Invoice.";
            public const string EditingFailure = "Fatal error occurred while editing the Invoice.";
            public const string DeletionFailure = "Fatal error occurred while deleting the Invoice.";
            public const string RetrievingFailure = "Fatal error occurred while retrieving the Invoices.";
            public const string GetModelFailure = "Fatal error occurred while getting the Invoice.";

            public const string CannotCreateInvoiceWithoutProducts = "Invoice must contain at least one product.";
            public const string InvalidDate = "Cannot set import invoice date later than export invoice date: ";
            public const string ExistingExportInvoices = "Cannot delete an invoice with existing exports.";
        }

        public static class ImportInvoiceDetail
        {
            public const string QuantityRequired = "Quantity is required.";
            public const string QuantityRange = "Quantity must be greater than 0.";
            public const string PriceRange = "Price must be a positive number.";
            public const string ProductDuplicate = "Cannot have more than one product with the same name and category.";
            public const string ProductNotFound = "Product not found.";
            public const string CreationFailure = "Failed to create or update ImportInvoiceDetail.";
            public const string ProductDeletionFailure = "Products that are used in export invoices cannot be removed.";
            public const string DeletionFailure = "Fatal error occurred while removing the products.";
            public const string CannnotDeleteAllProducts = "Cannot delete all products.";
        }

        public static class ExportInvoice
        {
            public const string InvoiceNumberRequired = "Invoice number is required.";
            public const string InvoiceNumberMaxLength = "Invoice number cannot be longer than 20 characters.";
            public const string InvoiceNumberMinLength = "Invoice number must be at least 3 characters.";
            public const string DateRequired = "Date is required.";
            public const string DuplicateInvoice = "An Export Invoice with this number already exists.";
            public const string NoPermissionOrImportInvoiceNotFound = "Import Invoice not found or access denied.";
            public const string NoPermissionOrExportInvoiceNotFound = "Export Invoice not found or access denied.";
            public const string InsufficientStock = "Insufficient stock.";
            public const string CannotExportBeforeImportDate = "Cannot export before import date.";
            public const string DuplicateProduct = "Duplicate product entries from the same import invoice are not allowed.";

            public const string CreationFailure = "Fatal error occurred while creating the Export Invoice.";
            public const string EditingFailure = "Fatal error occurred while editing the Export Invoice.";
            public const string DeletionFailure = "Fatal error occurred while deleting the Export Invoice.";
            public const string RetrievingFailure = "Fatal error occurred while retrieving the Export Invoices.";
            public const string GetModelFailure = "Fatal error occurred while getting the Export Invoice.";

            public const string CannotCreateExportInvoiceWithoutExports = "Export Invoice must contain at least one exported product.";
            public const string InvalidDate = "Cannot set Export Invoice date earlier than invoice date: ";
        }

        public static class ExportInvoiceDetail
        {
            public const string ImportInvoiceNumberRequired = "Import invoice number is required.";
            public const string QuantityRequired = "Quantity is required.";
            public const string QuantityRange = "Quantity must be greater than 0.";
            public const string PriceRange = "Price must be a positive number.";
            public const string ProductNotFoundInImportInvoice = "Product not found - please check the product name, category, and invoice number.";
            public const string ExportNotFound = "Export not found.";
            public const string CreationFailure = "Failed to create or update ExportInvoiceDetail.";
            public const string ExportDuplicate = "Cannot have more than one export with the same product name, category and Import Invoice.";
            public const string DeletionFailure = "Fatal error occurred while removing the exports.";
            public const string CannnotDeleteAllExports = "Cannot delete all exports.";
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

        public static class Product
        {
            public const string ProductNameRequired = "Product name is required.";
            public const string ProductNameMinLength = "Product name must be at least 3 characters.";
            public const string ProductNameMaxLength = "Product name cannot be longer than 255 characters.";
            public const string ProductDescriptionMaxLength = "Product description cannot be longer than 1000 characters.";
            public const string ProductDescriptionMinLength = "Product description must be at least 5 characters.";
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

            public const string ClientNameRequired = "Client name is required.";
            public const string ClientNameMinLength = "Client name must be at least 3 characters.";
            public const string ClientNameMaxLength = "Client name cannot be longer than 150 characters.";

            public const string ClientAddressRequired = "Client address is required.";
            public const string ClientAddressMinLength = "Client address must be at least 5 characters.";
            public const string ClientAddressMaxLength = "Client address cannot be longer than 150 characters.";

            public const string ClientPhoneNumberMinLength = "Phone number must be at least 10 characters.";
            public const string ClientPhoneNumberMaxLength = "Phone number cannot be longer than 30 characters.";
            public const string ClientPhoneNumberInvalid = "Invalid phone number format.";

            public const string ClientEmailMinLength = "Email must be at least 5 characters.";
            public const string ClientEmailMaxLength = "Email cannot be longer than 100 characters.";
            public const string ClientEmailInvalid = "Invalid email address format.";
        }

        public static class Stock
        {
            public const string ProductNotFound = "Product not found.";
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
