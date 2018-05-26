using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Auth;
using System;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class DentistController : Controller
    {
        private DentistsDAO DentistsDAO;

        IConfiguration config;

        public DentistController(IConfiguration _config)
        {
            config = _config;
            DentistsDAO = new DentistsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
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

            var dentist = new Dentist();

            return View(dentist);
        }

        [HttpPost]
        public IActionResult Add(Dentist Model)
        {
            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            int dentistId = DentistsDAO.Add(Model);

            if (dentistId == 0)
            {
                ViewData["Section"] = "Dentistas";
                ViewData["Action"] = "Criar Novo";
                return View(Model);
            }

            return RedirectToAction("Details", new { id = dentistId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Editar";

            var dentist = DentistsDAO.GetById(id);

            return View("Add", dentist);
        }

        [HttpPost]
        public IActionResult Edit(Dentist Model)
        {
            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            DentistsDAO.Edit(Model);

            return RedirectToAction("Details", new { id = Model.ID });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["Section"] = "Dentistas";
            ViewData["Action"] = "Detalhes";

            var dentist = DentistsDAO.GetById(id);

            if (dentist == null)
                return RedirectToAction("Index", "Dashboard");

            return View(dentist);
        }
    }
}