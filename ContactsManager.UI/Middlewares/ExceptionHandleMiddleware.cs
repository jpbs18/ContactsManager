using Serilog;

namespace CrudExample.Middlewares
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger, IDiagnosticContext diagnosticContext)
        {
            _next = next;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task Invoke(HttpContext httpContext)
       {
            try
            {
              await _next(httpContext);
            }
            catch(Exception ex)
            {
                string exception;
                string message;

                if (ex.InnerException is not null)
                {
                    exception = ex.InnerException.GetType().ToString();
                    message = ex.InnerException.Message;
                    _logger.LogError("{ExceptionType}.{ExceptionMessage}", exception, message);
                }
                else
                {
                    exception = ex.GetType().ToString();
                    message = ex.Message;
                    _logger.LogError("{ExceptionType}.{ExceptionMessage}", exception, message);
                }

                //httpContext.Response.StatusCode = 500;
                //await httpContext.Response.WriteAsJsonAsync(new { exception, message });
                throw;
            }
            
        }
    }

    
    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandleMiddleware>();
        
    }
}
