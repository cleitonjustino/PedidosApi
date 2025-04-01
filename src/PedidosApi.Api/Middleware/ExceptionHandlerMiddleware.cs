using System.Net;
using System.Text.Json;
using FluentValidation;

namespace PedidosApi.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
        catch (ValidationException ex) // Adicionando captura para ValidationException
        {
            _logger.LogError(ex, $"Falha de validação: {ex.Message}");
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest); // Retorno com BadRequest
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, $"Falha no envio: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = context.Response.StatusCode,
            message = exception is ValidationException ? "Erro de validação" : "Ocorreu um erro interno no servidor",
            detalhes = exception.Message
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}