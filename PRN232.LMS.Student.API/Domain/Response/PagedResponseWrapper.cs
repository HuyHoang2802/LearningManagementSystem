using System.Text.Json.Serialization;

namespace PRN232.LMS.Student.API.Domain.Response;

public class PagedResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("pagination")]
    public PRN232.LMS.Student.API.Domain.Response.PagedResponse? Pagination { get; set; }

    public PagedResponse() { }

    public PagedResponse(bool success, string message, T? data = default, PRN232.LMS.Student.API.Domain.Response.PagedResponse? pagination = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Pagination = pagination;
    }
}