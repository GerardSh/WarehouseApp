namespace WarehouseApp.Common.Constants
{
    public static class EntityConstants
    {
        public const string MoneyType = "decimal(18,2)";

        public static class ImportInvoice
        {
            public const int InvoiceNumberMinLength = 3;

            public const int InvoiceNumberMaxLength = 20;
        }

        public static class ExportInvoice
        {
            public const int InvoiceNumberMinLength = 3;

            public const int InvoiceNumberMaxLength = 20;
        }

        public static class Client
        {
            public const int NameMinLength = 3;

            public const int NameMaxLength = 150;

            public const int AddressMinLength = 5;

            public const int AddressMaxLength = 150;

            public const int PhoneNumberMinLength = 10;

            public const int PhoneNumberMaxLength = 30;

            public const int EmailMinLength = 5;

            public const int EmailMaxLength = 100;
        }

        public static class Warehouse
        {
            public const int NameMinLength = 5;

            public const int NameMaxLength = 150;

            public const int AddressMinLength = 5;

            public const int AddressMaxLength = 150;
        }

        public static class Product
        {
            public const int NameMinLength = 3;

            public const int NameMaxLength = 255;

            public const int DescriptionMinLength = 5;

            public const int DescriptionMaxLength = 1000;
        }

        public static class Category
        {
            public const int NameMinLength = 3;

            public const int NameMaxLength = 255;

            public const int DescriptionMinLength = 5;

            public const int DescriptionMaxLength = 1000;
        }
    }
}
