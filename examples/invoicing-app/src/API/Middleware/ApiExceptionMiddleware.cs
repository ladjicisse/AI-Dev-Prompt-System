using System.Text.Json;

namespace LADCI.AI.Invoicing.API.Middleware;

internal sealed class ApiExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException exception) when (exception.Message == "Invoice was not found.")
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                type = "https://httpstatuses.com/404",
                title = "Not Found",
                status = StatusCodes.Status404NotFound,
                detail = exception.Message
            }));
        }
    }
}
