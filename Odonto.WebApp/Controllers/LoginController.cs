using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.WebApp.Helpers.Filters;
using System.Reflection;

namespace Odonto.WebApp.Controllers
{
    public class LoginController : Controller
    {
        private UsersDAO UsersDAO;
        private PersonsDAO PersonsDAO;

        IConfiguration config;

        public LoginController(IConfiguration _config)
        {
            config = _config;
            UsersDAO = new UsersDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            PersonsDAO = new PersonsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet, TypeFilter(typeof(IsNotLoggedAttribute))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, TypeFilter(typeof(IsNotLoggedAttribute))]
        public IActionResult Index(string email, string password)
        {
            var user = UsersDAO.GetByEmail(email);

            if (user != null)
            {
                var person = PersonsDAO.GetById(user.ID);
                HttpContext.Session.SetInt32("userId", user.ID);
                HttpContext.Session.SetString("userEmail", user.Email);
                HttpContext.Session.SetString("userType", user.Type);
                HttpContext.Session.SetString("userName", person.Name);
                HttpContext.Session.SetInt32("clinicId", person.ClinicID);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "E-mail ou senha inválidos";
            return View();
        }

        [HttpGet, TypeFilter(typeof(IsLoggedAttribute))]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}