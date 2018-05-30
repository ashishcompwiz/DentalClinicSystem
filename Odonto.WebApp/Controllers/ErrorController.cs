using Microsoft.AspNetCore.Mvc;
using Odonto.WebApp.Helpers.Filters;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class ErrorController : Controller
    {
        public IActionResult Exception()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}