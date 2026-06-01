namespace HomeMaintenanceAPI.Application.Common
{
    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }

        public static ServiceResult Success()
        {
            return new ServiceResult
            {
                Succeeded = true
            };
        }

        public static ServiceResult Failure(string error)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Error = error
            };
        }
    }


    public class ServiceResult<T>
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string error)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Error = error
            };
        }
    }

}