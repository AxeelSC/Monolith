using HexagonalModular.API.Common;

namespace HexagonalModular.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.TraceIdentifier;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception in {Method} {Path}. TraceId: {TraceId}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    traceId);

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var response = ApiResponse<object>.ErrorResult(
                        errorCode: "SYSTEM.UNEXPECTED_ERROR",
                        message: "Internal Server Error",
                        traceId: traceId
                    );

                    await context.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}
