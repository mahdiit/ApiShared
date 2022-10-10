using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ApiShared.Core.Security
{
    public class XSecurityFilter : Attribute, IAuthorizationFilter
    {
        ISecurityHeaderValidator Validator;
        public XSecurityFilter(ISecurityHeaderValidator headerValidator)
        {
            Validator = headerValidator;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var attributes = descriptor.MethodInfo.CustomAttributes;
            if (attributes.Any(a => a.AttributeType == typeof(SkipXSecurityFilter)))
            {
                return;
            }

            bool tryCheck = (attributes.Any(a => a.AttributeType == typeof(TryXSecurityFilter)));
            var header = context.HttpContext.Request.Headers[StaticParameters.Security.ApiHeader.Name];

            if (string.IsNullOrEmpty(header) && attributes.Any(a => a.AttributeType == typeof(QueryCheckSecurityFilter)))
            {
                header = context.HttpContext.Request.Query[StaticParameters.Security.ApiHeader.QueryKey];
            }

            bool isValid = false;
            if (!string.IsNullOrEmpty(header))
            {
                Dictionary<string, string> headers;
                isValid = Validator.IsValid(header, out headers);

                if (isValid)
                {
                    foreach (var headerKey in headers.Keys)
                    {
                        context.HttpContext.Request.Headers[headerKey] = headers[headerKey];
                    }
                }
            }

            if (!tryCheck)
            {
                if (!isValid && !System.Net.IPAddress.IsLoopback(context.HttpContext.Connection.RemoteIpAddress))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new JsonResult(new
                    {
                        ErrorCode = 404,
                        Message = "شما به این بخش دسترسی ندارید"
                    });
                    return;
                }
            }
        }
    }
}
