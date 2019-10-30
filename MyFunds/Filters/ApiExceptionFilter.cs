using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyFunds.Extensions;
using MyFunds.Library.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFunds.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public ApiExceptionFilter()
        {
        }

        public override void OnException(ExceptionContext context)
        {

            if (context.Exception is UnauthorizedAccessException)
            {
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Title = "Unauthorized Access",
                    Status = 401,
                    Detail = "The instance value should be used to identify the problem when calling customer support",
                    Instance = $"urn:MyFunds:unauthorised:{Guid.NewGuid()}"
                };
                context.HttpContext.Response.StatusCode = 401;
                context.HttpContext.Response.WriteJson(problemDetails, "application/problem+json");

                LogError(problemDetails, context);
            }

            else if (context.Exception is ApiException)
            {
                var ex = context.Exception as ApiException;
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Title = "An error occured while processing Api request",
                    Status = 400,
                    Detail = $"{ex.Message}" +
                    " - The instance value should be used to identify the problem when calling customer support",
                    Instance = $"urn:MyFunds:badrequest:{Guid.NewGuid()}"
                };
                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.WriteJson(problemDetails, "application/problem+json");

                LogError(problemDetails, context);
            }

            else if (context.Exception is NoDataException)
            {
                var ex = context.Exception as NoDataException;
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Title = "Unable to return requested data",
                    Status = 404,
                    Detail = $"{ex.Message}" +
                    " - The instance value should be used to identify the problem when calling customer support",
                    Instance = $"urn:MyFunds:notfound:{Guid.NewGuid()}"
                };
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.WriteJson(problemDetails, "application/problem+json");

                LogError(problemDetails, context);
            }


            base.OnException(context);
        }


        private void LogError(ProblemDetails problemDetails, ExceptionContext context)
        {
            // TODO: replace with logger
            Debug.WriteLine($"\nTitle: {problemDetails.Title}" +
                        $"\nStatus: {problemDetails.Status}" +
                        $"\nInstance: {problemDetails.Instance}" +
                        $"\nDetails: {problemDetails.Detail}" +
                        $"\nPath: {context.HttpContext.Request.Path.Value}" +
                        $"\nQueryString: {context.HttpContext.Request.QueryString}" +
                        $"\nStackTrace: {context.Exception.Demystify().ToString()}" +
                        $"\nOriginalException: {context.Exception.ToString()}");

            if (context.Exception.Data != null && context.Exception.Data.Count > 0)
            {
                var exceptionData = new StringBuilder();
                foreach (DictionaryEntry d in context.Exception.Data)
                {
                    exceptionData.Append($"\nKey: {d.Key.ToString()} Value: {d.Value}");
                }
                Debug.WriteLine($"ExceptionData: {exceptionData}");
            }
        }


    }
}
