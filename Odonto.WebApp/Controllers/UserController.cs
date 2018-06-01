using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Filters;
using System;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute)), CheckAccess("ADMIN")]
    public class UserController : Controller
    {
        private UsersDAO UsersDAO;

        IConfiguration config;

        public UserController(IConfiguration _config)
        {
            config = _config;
            UsersDAO = new UsersDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult List()
        {
            ViewData["Section"] = "Usuários";
            ViewData["Action"] = "Listar";

            var userList = UsersDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(userList);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Usuários";
            ViewData["Action"] = "Editar";
            ViewBag.Types = UsersDAO.GetTypes();

            var user = UsersDAO.GetById(id);

            return View("Add", user);
        }

        [HttpPost]
        public IActionResult Edit(User Model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Section"] = "Usuários";
                ViewData["Action"] = "Editar";
                ViewBag.Types = UsersDAO.GetTypes();
                return View("Add", Model);
            }
            UsersDAO.Edit(Model);

            return RedirectToAction("List");
        }
    }
}