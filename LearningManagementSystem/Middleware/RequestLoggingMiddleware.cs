using System.Diagnostics;

namespace PRN232.LMS.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "Request: {Method} {Path} executed in {ExecutionTime}ms with response status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                executionTime,
                context.Response.StatusCode);
        }
    }
}
