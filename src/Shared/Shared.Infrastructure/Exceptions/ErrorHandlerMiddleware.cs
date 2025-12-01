using System.Collections.Concurrent;
using Shared.Abstractions.Exceptions;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Exceptions;

internal class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ConcurrentDictionary<string, string> _exceptionCodes = new();
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = 500;
        var code = "error";
        var message = "There was an error";

        _logger.LogError(ex, ex.Message);

        if (ex is CustomException customException)
        {
            statusCode = 400;
            message = customException.Message;
            code = GetExceptionCode(customException);
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { code, message });
    }

    private string GetExceptionCode(CustomException customException)
    {
        var exceptionCode = customException.GetType().Name;

        if (_exceptionCodes.TryGetValue(exceptionCode, out var errorCode)) return errorCode;
        
        errorCode = customException.GetType().Name.Underscore().Replace("_exception", "");
        _exceptionCodes.TryAdd(exceptionCode, errorCode);

        return errorCode;
    }
}