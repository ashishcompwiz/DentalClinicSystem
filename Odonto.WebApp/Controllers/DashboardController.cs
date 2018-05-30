using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.WebApp.Helpers.Filters;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class DashboardController : Controller
    {
        private DentistsDAO DentistsDAO;
        private PatientsDAO PatientsDAO;

        IConfiguration config;

        public DashboardController(IConfiguration _config)
        {
            config = _config;
            DentistsDAO = new DentistsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
            PatientsDAO = new PatientsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        public IActionResult Index()
        {
            ViewData["Section"] = "Dashboard";

            ViewBag.Dentists = DentistsDAO.Length();
            ViewBag.Patients = PatientsDAO.Length();

            return View();
        }
    }
}