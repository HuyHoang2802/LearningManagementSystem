using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace PRN232.LMS.Student.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unhandled exception occurred in Student API.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An internal server error occurred.";

            if (exception is ArgumentException || exception is InvalidOperationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var response = new 
            {
                success = false,
                message = message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}