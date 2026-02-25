using FluentValidation;
using Microsoft.AspNetCore.Http;
using Sentinel.Api.Application.Exceptions;
using Sentinel.Api.Infrastructure.Exceptions;
namespace Sentinel.Api.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            UnauthorizedException ex => (StatusCodes.Status401Unauthorized, ex.Message),
            BadRequestException ex => (StatusCodes.Status400BadRequest, ex.Message),
            BadValidationRequest ex => (StatusCodes.Status400BadRequest, ex.Message),
            ForbiddenException ex => (StatusCodes.Status403Forbidden, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new { Error = message });
    }
}