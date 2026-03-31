using System.Diagnostics;
using FluentValidation;
using MusicApp.Application.Common.DTOs;
using MusicApp.Domain.Exceptions;

namespace MusicApp.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            await WriteResponse(context, "Validation failed.", errors);
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = 401;
            await WriteResponse(context, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            context.Response.StatusCode = 403;
            await WriteResponse(context, ex.Message);
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = 409;
            await WriteResponse(context, ex.Message);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await WriteResponse(context, ex.Message);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = 422;
            await WriteResponse(context, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            await WriteResponse(context, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, string message, object? errors = null)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse.Fail(message, errors);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        await context.Response.WriteAsJsonAsync(new
        {
            response.Success,
            response.Data,
            response.Message,
            response.Errors,
            traceId
        });
    }
}
