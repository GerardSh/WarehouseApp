namespace WarehouseApp.Common.Constants
{
    public static class ApplicationConstants
    {
        public const int ReleaseYear = 2025;

        public const string AdminArea = "Admin";

        public const string WarehouseFirst = "b689e5b1-8c23-462d-b931-97a7d2b40470";
        public const string WarehouseSecond = "be8f00a5-682d-4b43-9734-d3e17078cb52";

        public static readonly IReadOnlyList<string> WarehouseIds = new List<string>
        {
            WarehouseFirst,
            WarehouseSecond
        }.AsReadOnly();
    }
}
