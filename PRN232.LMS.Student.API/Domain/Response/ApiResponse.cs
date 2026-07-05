using System.Text.Json.Serialization;

namespace PRN232.LMS.Student.API.Domain.Response
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        public object? Errors { get; set; }

        [JsonPropertyName("pagination")]
        public PagedResponse? Pagination { get; set; }

    
        public ApiResponse()
        {
            Success = true;
            Message = string.Empty;
        }

        
        public ApiResponse(bool success, string message, T? data = default, object? errors = null, PagedResponse? pagination = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            Pagination = pagination;
        }

        
        public static ApiResponse<T> Ok(T data, string message = "Success", PagedResponse? pagination = null)
        {
            return new ApiResponse<T>(true, message, data, null, pagination);
        }

        public static ApiResponse<T> Error(string message, object? errors = null)
        {
            return new ApiResponse<T>(false, message, default, errors);
        }
    }
}