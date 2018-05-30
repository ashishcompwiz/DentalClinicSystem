using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Odonto.WebApp.Helpers.Filters
{
    public class IsLoggedAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectResult("~/Login");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do : after the action executes  
        }
    }
}