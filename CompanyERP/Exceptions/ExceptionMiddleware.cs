using System.Net;
using System.Text.Json;

namespace CompanyERP.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (Exception ex) 
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound; // (404) إذا كان الموظف أو الفرع غير موجود
                    break;

                case MaxWorkHoursExceededException:
                    statusCode = HttpStatusCode.BadRequest; // (400) الإضافة الجديدة لخطأ الساعات البزنس
                    break;
                case BadRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new { message = message };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}