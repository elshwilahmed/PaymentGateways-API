using System.Net;

namespace SarahaAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke (HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Unhandled Exception!",
                    Detailes = e.Message
                };

                await context.Response.WriteAsJsonAsync(result);
            }


        }
    }
}
