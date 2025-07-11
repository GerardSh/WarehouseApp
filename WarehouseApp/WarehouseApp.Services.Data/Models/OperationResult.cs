namespace WarehouseApp.Services.Data.Models
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static OperationResult<T> Failure(string message) =>
            new OperationResult<T> { Success = false, ErrorMessage = message };

        public static OperationResult<T> Ok(T? data = default) =>
            new OperationResult<T> { Success = true, Data = data };
    }
}