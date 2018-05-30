using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Auth;
using Odonto.WebApp.ViewModels;
using System;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class DentistController : Controller
    {
        private DentistsDAO DentistsDAO;
        private UsersDAO UsersDAO;

        IConfiguration config;

        public DentistController(IConfiguration _config)
        {
            config = _config;
            DentistsDAO = new DentistsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            UsersDAO = new UsersDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult List()
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Listar";

            var dentistList = DentistsDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(dentistList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Criar Novo";
            ViewBag.Types = UsersDAO.GetTypes();

            var ViewModel = new VMDentistUser();
            ViewModel.Dentist = new Dentist();
            ViewModel.User = new User();
            ViewModel.User.Type = "DENTIST";

            return View(ViewModel);
        }

        [HttpPost]
        public IActionResult Add(VMDentistUser ViewModel)
        {
            string error = string.Empty;

            if (UsersDAO.EmailRepeated(ViewModel.User))
                error += "Este e-mail já está sendo utilizado por outro usuário do sistema;";

            if (string.IsNullOrEmpty(ViewModel.User.Password))
                error += "Informe uma senha de acesso para o dentista;";

            ViewModel.Dentist.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            int dentistId = DentistsDAO.Add(ViewModel.Dentist);
            if (dentistId == -1)
                error += "Este CPF já foi cadastrado;";

            if (!string.IsNullOrEmpty(error))
            {
                ViewData["Section"] = "Dentistas";
                ViewData["Action"] = "Criar Novo";
                ViewBag.Types = UsersDAO.GetTypes();
                ViewBag.Error = error;
                return View(ViewModel);
            }

            ViewModel.User.ID = dentistId;
            UsersDAO.Add(ViewModel.User);

            return RedirectToAction("Details", new { id = dentistId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Editar";

            var ViewModel = new VMDentistUser();
            ViewModel.Dentist = DentistsDAO.GetById(id);
            ViewModel.User = UsersDAO.GetById(id);
            ViewBag.Types = UsersDAO.GetTypes();

            return View("Add", ViewModel);
        }

        [HttpPost]
        public IActionResult Edit(VMDentistUser ViewModel)
        {
            string error = string.Empty;
            if (UsersDAO.EmailRepeated(ViewModel.User))
                error += "Este e-mail já está sendo utilizado por outro usuário do sistema;";

            ViewModel.Dentist.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            if (DentistsDAO.Edit(ViewModel.Dentist) <= 0)
                error += "Este e-mail já está sendo utilizado por outro usuário do sistema;";

            if (!string.IsNullOrEmpty(error))
            {
                ViewData["Section"] = "Dentistas";
                ViewData["Action"] = "Editar";
                ViewBag.Types = UsersDAO.GetTypes();
                ViewBag.Error = error;
                return View("Add", ViewModel);
            }

            UsersDAO.Edit(ViewModel.User);

            return RedirectToAction("Details", new { id = ViewModel.Dentist.ID });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Detalhes";

            var ViewModel = new VMDentistUser();
            ViewModel.Dentist = DentistsDAO.GetById(id);
            ViewModel.User = UsersDAO.GetById(id);

            if (ViewModel.Dentist == null)
                return RedirectToAction("Index", "Dashboard");

            return View(ViewModel);
        }
    }
}