using ECommerceCore.Application.Common;
using ECommerceCore.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace ECommerceCore.Api.MIddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> loger)
        {
            _next = next;
            _logger = loger;

        }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);


            }






        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Something went wrong";
            object errors = null;

            switch (ex)
            {

                case AppException appexception:
                    statusCode = appexception.StatusCode;
                    message = appexception.Message;
                    break;
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = "Resource not found";
                    break;

                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Unauthorized";
                    break;







            }

            var responce = ApiResponse<object>.Failure(message, errors);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(responce));

        }
    }
}
