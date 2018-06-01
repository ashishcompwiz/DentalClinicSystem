using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Filters;
using System;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class ProceduresController : Controller
    {
        private ProceduresDAO ProceduresDAO;

        IConfiguration config;

        public ProceduresController(IConfiguration _config)
        {
            config = _config;
            ProceduresDAO = new ProceduresDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult List()
        {
            ViewData["Section"] = "Procedimentos";
            ViewData["Action"] = "Listar";

            var procedures = ProceduresDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(procedures);
        }

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult Add()
        {
            ViewData["Section"] = "Procedimentos";
            ViewData["Action"] = "Criar Novo";

            var procedure = new Procedure();

            return View(procedure);
        }

        [HttpPost, CheckAccess("ADMIN")]
        public IActionResult Add(Procedure Model)
        {
            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            if (!ProceduresDAO.Add(Model))
            {
                ViewData["Section"] = "Procedimentos";
                ViewData["Action"] = "Criar Novo";
                ViewBag.Error = "Erro ao salvar o procedimento";
                return View(Model);
            }

            return RedirectToAction("List");
        }

        [HttpGet, CheckAccess("ADMIN")]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Procedimentos";
            ViewData["Action"] = "Editar";

            var procedure = ProceduresDAO.GetById(id);

            return View("Add", procedure);
        }

        [HttpPost, CheckAccess("ADMIN")]
        public IActionResult Edit(Procedure Model)
        {
            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            if (!ProceduresDAO.Edit(Model))
            {
                ViewData["Section"] = "Procedimentos";
                ViewData["Action"] = "Editar";
                ViewBag.Error = "Erro ao editar o procedimento";
                return View("Add", Model);
            }

            return RedirectToAction("Details", new { id = Model.ID });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["Section"] = "Procedimentos";
            ViewData["Action"] = "Detalhes";

            var procedure = ProceduresDAO.GetById(id);

            if (procedure == null)
                return RedirectToAction("Index", "Dashboard");

            return View(procedure);
        }
    }
}