namespace WarehouseApp.Services.Data.Models
{
    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }

        public new static OperationResult<T> Failure(string message) =>
            new OperationResult<T> { Success = false, ErrorMessage = message };

        public static OperationResult<T> Ok(T? data = default) =>
            new OperationResult<T> { Success = true, Data = data };
    }
}