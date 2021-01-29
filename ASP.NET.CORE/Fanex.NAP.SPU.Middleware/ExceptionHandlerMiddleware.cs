using Fanex.NAP.SPU.Middleware.Models;
using Microsoft.AspNetCore.Http;
using Sentry;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);

                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails(
                statusCode: context.Response.StatusCode,
                message: "Internal Server Error from the custom middleware.").ToString());
        }
    }
}
