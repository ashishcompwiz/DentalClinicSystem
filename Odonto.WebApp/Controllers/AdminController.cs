using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Filters;
using Odonto.WebApp.ViewModels;
using System;
using System.Linq;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class AdminController : Controller
    {
        private AdministratorsDAO AdministratorsDAO;
        private UsersDAO UsersDAO;

        IConfiguration config;

        public AdminController(IConfiguration _config)
        {
            config = _config;
            AdministratorsDAO = new AdministratorsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            UsersDAO = new UsersDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult List()
        {
            ViewData["Section"] = "Colaboradores";
            ViewData["Action"] = "Listar";

            var adminList = AdministratorsDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(adminList);
        }

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult Add()
        {
            ViewData["Section"] = "Colaboradores";
            ViewData["Action"] = "Criar Novo";
            ViewBag.Types = UsersDAO.GetTypes().Where(t => t.Name != "DEV" && t.Name != "DENTIST");

            var ViewModel = new VMAdminUser();
            ViewModel.Admin = new Administrator();
            ViewModel.User = new User();

            return View(ViewModel);
        }

        [HttpPost, CheckAccess("ADMIN")]
        public IActionResult Add(VMAdminUser ViewModel)
        {
            ViewModel.Admin.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            var adminId = AdministratorsDAO.Add(ViewModel.Admin);
            if (adminId > 0)
            {
                ViewModel.User.ID = adminId;
                UsersDAO.Add(ViewModel.User);
                return RedirectToAction("Details", new { id = adminId });
            }

            ViewBag.Types = UsersDAO.GetTypes().Where(t => t.Name != "DEV" && t.Name != "DENTIST");
            return View(ViewModel);
        }

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Colaboradores";
            ViewData["Action"] = "Editar";

            var ViewModel = new VMAdminUser();
            ViewModel.Admin = AdministratorsDAO.GetById(id);
            ViewModel.User = UsersDAO.GetById(id);
            ViewBag.Types = UsersDAO.GetTypes().Where(t => t.Name != "DEV" && t.Name != "DENTIST");
            var types = UsersDAO.GetTypes().Where(t => t.Name != "DEV" && t.Name != "DENTIST");

            return View("Add", ViewModel);
        }

        [HttpPost, CheckAccess("ADMIN")]
        public IActionResult Edit(VMAdminUser ViewModel)
        {
            ViewModel.Admin.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            if (AdministratorsDAO.Edit(ViewModel.Admin) > 0)
            {
                UsersDAO.Edit(ViewModel.User);
                return RedirectToAction("Details", new { id = ViewModel.Admin.ID });
            }

            ViewBag.Types = UsersDAO.GetTypes().Where(t => t.Name != "DEV" && t.Name != "DENTIST");
            return View("Add", ViewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["Section"] = "Colaboradores";
            ViewData["Action"] = "Detalhes";

            var ViewModel = new VMAdminUser();
            ViewModel.Admin = AdministratorsDAO.GetById(id);
            ViewModel.User = UsersDAO.GetById(id);

            if (ViewModel.Admin == null)
                return RedirectToAction("Index", "Dashboard");

            return View(ViewModel);
        }
    }
}