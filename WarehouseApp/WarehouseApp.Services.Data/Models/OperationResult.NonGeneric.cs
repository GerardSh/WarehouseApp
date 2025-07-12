namespace WarehouseApp.Services.Data.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static OperationResult Failure(string message) =>
            new OperationResult { Success = false, ErrorMessage = message };

        public static OperationResult Ok() =>
            new OperationResult { Success = true, };
    }
}