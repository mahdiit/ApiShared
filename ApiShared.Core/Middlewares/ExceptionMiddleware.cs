using ApiShared.Core.Data.BaseClass;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, Logger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AppException ex)
            {
                //do nothing
                await HandleExceptionAsync(httpContext, ex);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMiddleware", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string errorMsg = "خطا در دریافت اطلاعات";
            int errorCode = (int)HttpStatusCode.InternalServerError;

            if (exception is AppException)
            {
                errorMsg = exception.Message;
                var errCode = ((AppException)exception).ErrorCode;

                if (errCode != null)
                    errorCode = errCode.Value;
            }

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                new 
                {
                    ErrorCode = errorCode,
                    Message = errorMsg
                }));
        }
    }
}
