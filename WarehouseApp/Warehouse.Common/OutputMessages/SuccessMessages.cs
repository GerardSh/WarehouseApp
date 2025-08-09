namespace WarehouseApp.Common.OutputMessages
{
    public static class SuccessMessages
    {
        public static class Warehouse
        {
            public const string CreationSuccess = "The Warehouse was successfully created.";
            public const string EditingSuccess = "The Warehouse was successfully edited.";
            public const string DeletionSuccess = "The Warehouse was successfully deleted.";
        }

        public static class ImportInvoice
        {
            public const string CreationSuccess = "The Import Invoice was successfully created.";
            public const string EditingSuccess = "The Import Invoice was successfully edited.";
            public const string DeletionSuccess = "The Import Invoice was successfully deleted.";
        }

        public static class ExportInvoice
        {
            public const string CreationSuccess = "The Export Invoice was successfully created.";
            public const string EditingSuccess = "The Export Invoice was successfully edited.";
            public const string DeletionSuccess = "The Export Invoice was successfully deleted.";
        }

        public static class ExportData
        {
            public const string FetchedInvoices = "Fetched available invoice numbers.";
            public const string FetchedProducts = "Fetched products for invoice:";
        }

        public static class UserManager
        {
            public const string UserDeletionSuccess = "The selected user was successfully deleted.";
            public const string RoleRemovalSuccess = "The selected role was successfully removed.";
            public const string RoleAssignSuccess = "The selected role was successfully assigned.";
            public const string RequestSubmittedSuccess = "The request was successfully submitted.";
        }
    }
}
