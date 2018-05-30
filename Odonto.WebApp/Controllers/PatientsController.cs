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
    public class PatientsController : Controller
    {
        private DentistsDAO DentistsDAO;
        private ProceduresDAO ProceduresDAO;
        private DiseasesDAO DiseasesDAO;
        private PatientsDAO PatientsDAO;
        private PatientRecordDAO PatientRecordDAO;

        IConfiguration config;

        public PatientsController(IConfiguration _config)
        {
            config = _config;
            DentistsDAO = new DentistsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            ProceduresDAO = new ProceduresDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            DiseasesDAO = new DiseasesDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            PatientsDAO = new PatientsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            PatientRecordDAO = new PatientRecordDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult Records(int id)
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Prontuário Odontológico";

            var patient = PatientsDAO.GetById(id);
            if (patient == null)
                return RedirectToAction("Index", "Dashboard");

            var patientRecord = PatientRecordDAO.GetById(id);
            if (patientRecord != null)
            {
                patientRecord.Diseases = PatientRecordDAO.GetDiseases(id);
                patientRecord.Procedures = PatientRecordDAO.GetProcedures(id);
                patient.Record = patientRecord;
            }

            return View(patient);
        }

        [HttpGet]
        public IActionResult List(string name, string cpf)
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Listar";

            var patientList = PatientsDAO.GetFiltered(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")), name, cpf);

            ViewBag.cpf = cpf;
            ViewBag.name = name;

            return View(patientList);
        }

        #region CRUD
        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Criar Novo";

            var Model = new Patient();

            return View(Model);
        }

        [HttpPost]
        public IActionResult Add(Patient Model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Criar Novo";
                return View(Model);
            }

            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            Model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
            int id = PatientsDAO.Add(Model);
            if (id < 0)
            {
                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Criar Novo";
                ViewData["Error"] = "O CPF informado já foi cadastrado";
                return View(Model);
            }

            return RedirectToAction("Records", new { id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var patient = PatientsDAO.GetById(id);

            if (patient == null)
                return RedirectToAction("Index", "Dashboard");

            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Editar";

            return View("Add", patient);
        }

        [HttpPost]
        public IActionResult Edit(Patient Model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Editar";
                return View("Add", Model);
            }

            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            Model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
            if (PatientsDAO.Edit(Model) <= 0)
            {
                ViewBag.Error = "Este CPF já foi cadastrado";
                return View(Model);
            }

            return RedirectToAction("Records", new { id = Model.ID});
        }
        #endregion

        #region Procedure
        [HttpGet]
        public IActionResult AddProcedure(int id)
        {
            var patient = PatientsDAO.GetById(id);

            if (patient == null)
                return RedirectToAction("Index", "Dashboard");

            var procedure = new PatientRecordProcedure();
            procedure.PatientRecordID = id;

            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Criar Novo Procedimento";
            ViewBag.Procedures = ProceduresDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));
            ViewBag.Dentists = DentistsDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));
            ViewBag.PatientName = patient.Name;

            return View(procedure);
        }

        [HttpPost]
        public IActionResult AddProcedure(PatientRecordProcedure Model)
        {
            if (!ModelState.IsValid)
            {
                var patient = PatientsDAO.GetById(Model.PatientRecordID);

                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Criar Novo Procedimento";
                ViewBag.Procedures = ProceduresDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));
                ViewBag.Dentists = DentistsDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));
                ViewBag.PatientName = patient.Name;

                return View(Model);
            }

            PatientRecordDAO.AddProcedure(Model);

            return RedirectToAction("Records", new { id = Model.PatientRecordID });
        }
        #endregion

        #region Anamnesis
        [HttpGet]
        public IActionResult AddAnamnesis(int id)
        {
            var patient = PatientsDAO.GetById(id);

            if (patient == null)
                return RedirectToAction("Index", "Dashboard");

            var anamnese = PatientRecordDAO.GetById(id);
            if (anamnese == null)
            {
                anamnese = new PatientRecord();
                anamnese.PatientID = id;
            }

            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Criar Anamnese";
            ViewBag.PatientName = patient.Name;
            ViewBag.Diseases = DiseasesDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(anamnese);
        }

        [HttpPost]
        public IActionResult AddAnamnesis(PatientRecord Model, string[] diseases, string[] descriptions)
        {
            ViewBag.Diseases = DiseasesDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            if (diseases != null)
            {
                foreach(var item in diseases)
                {
                    var descriptionIndex = Convert.ToInt32(item.Substring(0, item.IndexOf('-')));
                    var diseaseId = Convert.ToInt32(item.Substring(item.IndexOf('-') + 1, item.Length - item.IndexOf('-') - 1));

                    Model.Diseases.Add(new PatientRecordDisease(Model.PatientID, diseaseId, descriptions[descriptionIndex]));
                }
            }

            PatientRecordDAO.Add(Model);
            PatientRecordDAO.AddDiseases(Model.Diseases);

            return RedirectToAction("Records", new { id = Model.PatientID });
        }
        #endregion
    }
}