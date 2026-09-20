namespace PaymentGateway.IntegrationAPI.Responces
{
    public class APIResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }

        public static APIResponse<T> Success(T data, string msg = "Operation completed successfully")
        {
            return new APIResponse<T>
            {
                IsSuccess = true,
                Message = msg,
                Data = data
            };
        }

        public static APIResponse<T> Failure(string msg = "")
        {
            return new APIResponse<T>
            {
                IsSuccess = false,
                Message = msg,
                Data = default
            };
        }
    }
}
