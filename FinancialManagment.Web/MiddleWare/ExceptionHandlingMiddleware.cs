using FinancialManagment.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FinancialManagment.Web.MiddleWare;

public sealed class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger,
    ITempDataDictionaryFactory tempDataFactory) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(ex, "Response already started. TraceId: {TraceId}", context.TraceIdentifier);
                throw;
            }


            MapException(ex, out int statusCode, out string message);

            logger.Log(
                GetLogLevel(statusCode),
                ex,
                "Handled exception. StatusCode: {StatusCode}, TraceId: {TraceId}, Path: {Path}, Message: {Message}",
                statusCode,
                context.TraceIdentifier,
                context.Request.Path.Value,
                ex.Message);

            WriteTempData(context, message, statusCode);

            context.Response.Clear();
            context.Response.StatusCode = statusCode;

            if (statusCode == StatusCodes.Status404NotFound)
            {
                context.Response.Redirect("/Home/NotFoundPage");
                return;
            }

            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            context.Response.Redirect("/Home/Error");
        }
    }

    private static void MapException(Exception ex, out int statusCode, out string message)
    {
        statusCode = StatusCodes.Status500InternalServerError;
        message = "Došlo k neočekávané chybě.";


        if (ex is NotFoundException nf)
        {
            statusCode = StatusCodes.Status404NotFound;
            message = nf.Message;
            return;
        }

        if (ex is ArgumentException ae)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = ae.Message;
        }

        if (ex is UnauthorizedAccessException uae)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            message = uae.Message;
        }
    }

    private void WriteTempData(HttpContext context, string message, int statusCode)
    {
        var tempData = tempDataFactory.GetTempData(context);
        tempData["Error"] = message;
        tempData["StatusCode"] = statusCode;
        tempData["TraceId"] = context.TraceIdentifier;
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        if (statusCode >= 500)
            return LogLevel.Error;

        return LogLevel.Warning;
    }
}
