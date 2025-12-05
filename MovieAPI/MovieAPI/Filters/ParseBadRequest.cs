using System.Collections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MovieAPI.Filters;

public class ParseBadRequest: IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var castResult = context.Result as IStatusCodeActionResult;
        
        if (castResult == null)
        {
            return;
        }
        var codeStatus = castResult.StatusCode;
        
        if (codeStatus == 400)
        {
            var response = new List<string>();
            var actualresponse = context.Result as BadRequestObjectResult;
            if (actualresponse.Value is string)
            {
                response.Add(actualresponse.Value as string);
            }
            else if (actualresponse.Value is IEnumerable<IdentityError> errors)
            {
                foreach (var error in errors)
                {
                    response.Add(error.Description);
                }
            }
            else
            {
                foreach (var key in context.ModelState.Keys)
                {
                    foreach (var error in context.ModelState[key].Errors)
                    {
                        response.Add($"{key} : {error.ErrorMessage}");
                    }
                }
            }
            
            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}