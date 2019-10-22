using Microsoft.AspNetCore.Mvc;
using MyFunds.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Exceptions
{
    public class ValidationProblemDetailsResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(m => m.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            var details = "See ValidationErrors for details";
            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty)
                {
                    details = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                }
                else
                {
                    var allErrors = modelStateEntries.SelectMany(entry => entry.Value.Errors,
                                                                (parent, child) => new ValidationError
                                                                {
                                                                    Name = parent.Key,
                                                                    Description = child.ErrorMessage
                                                                });
                    errors.AddRange(allErrors);
                }
            }

            var problemDetails = new ValidationProblemDetails
            {
                Title = "Request Validation Error",
                Status = 400,
                Detail = details,
                Instance = $"urn:TrackMe:badrequest:{Guid.NewGuid()}",
                ValidationErrors = errors
            };

            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.WriteJson(problemDetails, "application/problem+json");
            return Task.CompletedTask;
        }
    }
}
