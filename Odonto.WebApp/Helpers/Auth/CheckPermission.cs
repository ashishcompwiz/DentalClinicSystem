using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Odonto.WebApp.Helpers.Auth
{
    public class CheckPermission : ActionFilterAttribute
    {
        private readonly ILogger logger;
        private readonly string loggerName;

        public CheckPermission(ILogger logg, string loggName)
        {
            this.logger = logg;
            this.loggerName = loggName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectResult("~/Login");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //To do : after the action executes  
        }
    }
}
