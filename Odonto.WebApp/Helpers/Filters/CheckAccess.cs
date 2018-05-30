using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Odonto.WebApp.Helpers.Filters
{
    public class CheckAccessAttribute : TypeFilterAttribute
    {
        public CheckAccessAttribute(params string[] userTypes) : base(typeof(CheckAccessImpl))
        {
            Arguments = new object[] { userTypes };
        }

        private class CheckAccessImpl : IActionFilter
        {
            private readonly string[] _userTypes;
            private readonly ILogger _logger;

            public CheckAccessImpl(ILoggerFactory loggerFactory, string[] userTypes)
            {
                _userTypes = userTypes;
                _logger = loggerFactory.CreateLogger<CheckAccessAttribute>();
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                string userType = context.HttpContext.Session.GetString("userType");
                if (userType != "DEV" && _userTypes.Length > 0)
                {
                    if (!_userTypes.ToString().Contains(userType))
                        context.Result = new RedirectResult("/Error/AccessDenied");
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}
