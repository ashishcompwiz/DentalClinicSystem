using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Filters;
using System;

namespace Odonto.WebApp.Controllers
{
    public class UserController : Controller
    {
        private UsersDAO UsersDAO;

        IConfiguration config;

        public UserController(IConfiguration _config)
        {
            config = _config;
            UsersDAO = new UsersDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult List()
        {
            ViewData["Section"] = "Usuários";
            ViewData["Action"] = "Listar";

            var userList = UsersDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(userList);
        }

        //[HttpGet]
        //public IActionResult Add()
        //{
        //    ViewData["Section"] = "Usuários";
        //    ViewData["Action"] = "Criar Novo";
        //    ViewBag.Types = UsersDAO.GetTypes();

        //    var user = new User();

        //    return View(user);
        //}

        //[HttpPost]
        //public IActionResult Add(User Model)
        //{
           
        //    if (string.IsNullOrEmpty(Model.Password))
        //    {
        //        ViewData["Section"] = "Usuários";
        //        ViewData["Action"] = "Criar Novo";
        //        ViewBag.Types = UsersDAO.GetTypes();
        //        ViewBag.Error = "Digite uma senha";

        //        return View(Model);
        //    }

        //    var added = UsersDAO.Add(Model);
        //    if (added)
        //    {
        //        ViewData["Section"] = "Usuários";
        //        ViewData["Action"] = "Criar Novo";
        //        ViewBag.Types = UsersDAO.GetTypes();

        //        return View(Model);
        //    }

        //    return RedirectToAction("List");
        //}

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Usuários";
            ViewData["Action"] = "Editar";
            ViewBag.Types = UsersDAO.GetTypes();

            var user = UsersDAO.GetById(id);

            return View("Add", user);
        }

        [HttpPost, CheckAccess("ADMIN")]
        public IActionResult Edit(User Model)
        {
            UsersDAO.Edit(Model);

            return RedirectToAction("List");
        }

        //[HttpGet]
        //public IActionResult Details(int id)
        //{
        //    ViewData["Section"] = "Usuários";
        //    ViewData["Action"] = "Detalhes";

        //    var user = UsersDAO.GetById(id);

        //    if (user == null)
        //        return RedirectToAction("Index", "Dashboard");

        //    return View(user);
        //}
    }
}