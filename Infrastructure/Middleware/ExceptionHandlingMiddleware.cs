using System.Net;
using System.Text.Json;

namespace LocadoraVeiculosApi.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteResponse(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = context.Response.StatusCode,
            erro = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
